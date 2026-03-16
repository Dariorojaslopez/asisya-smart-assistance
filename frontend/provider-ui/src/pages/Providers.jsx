import { useState, useEffect, useMemo } from "react";
import {
  MapContainer,
  TileLayer,
  Marker,
  Popup,
  useMapEvents,
  useMap,
} from "react-leaflet";
import L from "leaflet";
import "leaflet/dist/leaflet.css";
import { getAssistanceTypes, optimizeProviders } from "../services/api";
import "./Providers.css";

const DEFAULT_CENTER = [4.6097, -74.0817];
const DEFAULT_ZOOM = 12;

// Iconos personalizados (círculos) para evitar problemas con rutas de Leaflet en bundlers
function createCircleIcon(color) {
  return L.divIcon({
    className: "custom-marker",
    html: `<div style="background-color:${color};width:24px;height:24px;border-radius:50%;border:2px solid white;box-shadow:0 1px 5px rgba(0,0,0,0.4);"></div>`,
    iconSize: [24, 24],
    iconAnchor: [12, 12],
  });
}

const CLIENT_ICON = createCircleIcon("#22c55e");   // green
const PROVIDER_ICON = createCircleIcon("#3b82f6"); // blue
const BEST_ICON = createCircleIcon("#eab308");     // gold

// Mapeo código del catálogo (id "1","2",...) al código que espera la API de optimización (GRUA, BATERIA, ...)
const CATALOG_CODE_TO_OPTIMIZE_CODE = {
  "1": "GRUA",
  "2": "BATERIA",
  "3": "COMBUSTIBLE",
  "4": "CERRAJERIA",
  "5": "LLANTA",
  "6": "MECANICA",
};

// Escucha clics en el mapa y actualiza lat/lng
function MapClickHandler({ onMapClick }) {
  useMapEvents({
    click(e) {
      const { lat, lng } = e.latlng;
      onMapClick(lat.toFixed(4), lng.toFixed(4));
    },
  });
  return null;
}

// Ajusta el zoom para mostrar todos los marcadores
function FitBounds({ clientPos, providers }) {
  const map = useMap();
  useEffect(() => {
    const positions = [];
    if (clientPos && !Number.isNaN(clientPos[0]) && !Number.isNaN(clientPos[1])) {
      positions.push(clientPos);
    }
    if (Array.isArray(providers) && providers.length > 0) {
      providers.forEach((p) => {
        const lat = p.latitud;
        const lng = p.longitud;
        if (lat != null && lng != null) positions.push([lat, lng]);
      });
    }
    if (positions.length === 0) return;
    if (positions.length === 1) {
      map.setView(positions[0], 14);
      return;
    }
    const bounds = L.latLngBounds(positions);
    map.fitBounds(bounds, { padding: [40, 40], maxZoom: 14 });
  }, [map, clientPos, providers]);
  return null;
}

export default function Providers() {
  const [assistanceTypes, setAssistanceTypes] = useState([]);
  const [latitude, setLatitude] = useState("4.6097");
  const [longitude, setLongitude] = useState("-74.0817");
  const [assistanceType, setAssistanceType] = useState("");
  const [providers, setProviders] = useState([]);
  const [loadingCatalog, setLoadingCatalog] = useState(true);
  const [loadingSearch, setLoadingSearch] = useState(false);
  const [error, setError] = useState(null);

  const clientPos = useMemo(() => {
    const lat = parseFloat(latitude);
    const lng = parseFloat(longitude);
    if (Number.isNaN(lat) || Number.isNaN(lng)) return null;
    return [lat, lng];
  }, [latitude, longitude]);

  const bestProviderId = useMemo(() => {
    if (!providers.length) return null;
    const best = providers.reduce((a, b) =>
      (a.etaMinutes ?? Infinity) <= (b.etaMinutes ?? Infinity) ? a : b
    );
    return best?.id ?? null;
  }, [providers]);

  useEffect(() => {
    getAssistanceTypes()
      .then((data) => {
        // El backend devuelve un diccionario { "1": "Grúa", "2": "Paso de corriente", ... }.
        // Lo convertimos a un array [{ code, name }] para poder mapear las opciones del <select>.
        const list = typeof data === "object" && data !== null && !Array.isArray(data)
          ? Object.entries(data).map(([code, name]) => ({ code, name: String(name) }))
          : Array.isArray(data)
            ? data
            : [];
        setAssistanceTypes(list);
        if (list.length > 0 && !assistanceType) {
          setAssistanceType(list[0].code);
        }
      })
      .catch((err) =>
        setError(err.response?.data?.message ?? err.message ?? "Error al cargar catálogo")
      )
      .finally(() => setLoadingCatalog(false));
  }, []);

  const handleMapClick = (lat, lng) => {
    setLatitude(String(lat));
    setLongitude(String(lng));
  };

  const handleSearch = async (e) => {
    e.preventDefault();
    setError(null);
    const lat = parseFloat(latitude);
    const lng = parseFloat(longitude);
    if (Number.isNaN(lat) || Number.isNaN(lng)) {
      setError("Indica una latitud y longitud válidas.");
      return;
    }
    if (!assistanceType?.trim()) {
      setError("Selecciona un tipo de asistencia.");
      return;
    }
    setLoadingSearch(true);
    try {
      // El catálogo usa códigos "1","2",...; el endpoint /optimize espera "GRUA","BATERIA",...
      const apiCode = CATALOG_CODE_TO_OPTIMIZE_CODE[assistanceType.trim()] ?? assistanceType.trim();
      const data = await optimizeProviders(lat, lng, apiCode);
      setProviders(Array.isArray(data) ? data : []);
    } catch (err) {
      setError(err.response?.data?.message ?? err.message ?? "Error al buscar proveedores");
      setProviders([]);
    } finally {
      setLoadingSearch(false);
    }
  };

  if (loadingCatalog) {
    return <p className="page-message">Cargando catálogo…</p>;
  }

  return (
    <div className="providers-page">
      <h1 className="page-title">Proveedores</h1>

      {/* Formulario de búsqueda */}
      <section className="providers-card providers-form-card">
        <h2 className="providers-card-title">Buscar proveedor óptimo</h2>
        <form onSubmit={handleSearch} className="providers-form">
          {error && <p className="providers-form-error">{error}</p>}
          <div className="providers-form-row">
            <label className="providers-label">
              Latitud
              <input
                type="text"
                value={latitude}
                onChange={(e) => setLatitude(e.target.value)}
                placeholder="ej. 4.6097"
                className="providers-input"
                disabled={loadingSearch}
              />
            </label>
            <label className="providers-label">
              Longitud
              <input
                type="text"
                value={longitude}
                onChange={(e) => setLongitude(e.target.value)}
                placeholder="ej. -74.0817"
                className="providers-input"
                disabled={loadingSearch}
              />
            </label>
          </div>
          <label className="providers-label">
            Tipo de asistencia
            <select
              value={assistanceType}
              onChange={(e) => setAssistanceType(e.target.value)}
              className="providers-select"
              disabled={loadingSearch}
            >
              <option value="">Selecciona tipo de asistencia</option>
              {assistanceTypes.map((t) => (
                <option key={t.code} value={t.code}>
                  {t.name}
                </option>
              ))}
            </select>
          </label>
          <button type="submit" className="providers-submit" disabled={loadingSearch}>
            {loadingSearch ? "Buscando…" : "Buscar proveedor óptimo"}
          </button>
        </form>
      </section>

      {/* Mapa interactivo */}
      <section className="providers-card providers-map-card">
        <h2 className="providers-card-title">Ubicación (clic en el mapa o edita lat/long)</h2>
        <div className="providers-map-wrap">
          <MapContainer
            center={DEFAULT_CENTER}
            zoom={DEFAULT_ZOOM}
            className="providers-map"
            zoomControl={true}
            scrollWheelZoom={true}
          >
            <TileLayer
              attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>'
              url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
            />
            <MapClickHandler onMapClick={handleMapClick} />
            <FitBounds clientPos={clientPos} providers={providers} />

            {clientPos && (
              <Marker position={clientPos} icon={CLIENT_ICON} title="Tu ubicación">
                <Popup>Cliente (tu ubicación)</Popup>
              </Marker>
            )}

            {providers.map((p) => (
              <Marker
                key={p.id}
                position={[p.latitud, p.longitud]}
                icon={p.id === bestProviderId ? BEST_ICON : PROVIDER_ICON}
                title={p.id}
              >
                <Popup>
                  <strong>Proveedor {p.id}</strong>
                  <br />
                  Rating: {p.calificacion}
                  <br />
                  ETA: {p.etaMinutes} min
                </Popup>
              </Marker>
            ))}
          </MapContainer>
        </div>
      </section>

      {/* Tabla de resultados */}
      <section className="providers-card providers-table-card">
        <h2 className="providers-card-title">Resultados</h2>
        <div className="table-wrap">
          <table className="data-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Latitude</th>
                <th>Longitude</th>
                <th>Rating</th>
                <th>ETA</th>
              </tr>
            </thead>
            <tbody>
              {providers.length === 0 ? (
                <tr>
                  <td colSpan={5} className="table-empty">
                    {loadingSearch
                      ? "Buscando…"
                      : "Usa el formulario y pulsa «Buscar proveedor óptimo» para ver resultados."}
                  </td>
                </tr>
              ) : (
                providers.map((p) => (
                  <tr key={p.id}>
                    <td>{p.id}</td>
                    <td>{p.latitud}</td>
                    <td>{p.longitud}</td>
                    <td>{p.calificacion}</td>
                    <td>{p.etaMinutes} min</td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </section>
    </div>
  );
}
