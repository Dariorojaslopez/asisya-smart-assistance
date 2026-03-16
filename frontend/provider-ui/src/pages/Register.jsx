import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { createUser } from "../services/api";
import "./Login.css";

export default function Register() {
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    if (!username.trim() || !email.trim() || !password) {
      setError("Completa todos los campos.");
      return;
    }
    if (password.length < 6) {
      setError("La contraseña debe tener al menos 6 caracteres.");
      return;
    }
    setLoading(true);
    try {
      await createUser({
        username: username.trim(),
        email: email.trim(),
        password,
        role: "Admin",
      });
      navigate("/login", { state: { message: "Cuenta creada. Inicia sesión." } });
    } catch (err) {
      const msg = err.response?.data?.message ?? err.response?.data ?? err.message;
      setError(typeof msg === "string" ? msg : "Error al crear la cuenta.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-page">
      <div className="login-card">
        <h1 className="login-title">Crear primera cuenta</h1>
        <p className="login-subtitle">Configura el usuario administrador inicial.</p>
        <form onSubmit={handleSubmit} className="login-form">
          {error && <p className="login-error">{error}</p>}
          <label className="login-label">
            Usuario
            <input
              type="text"
              value={username}
              onChange={(e) => { setUsername(e.target.value); setError(""); }}
              placeholder="Nombre de usuario"
              className="login-input"
              autoComplete="username"
              disabled={loading}
            />
          </label>
          <label className="login-label">
            Email
            <input
              type="email"
              value={email}
              onChange={(e) => { setEmail(e.target.value); setError(""); }}
              placeholder="correo@ejemplo.com"
              className="login-input"
              autoComplete="email"
              disabled={loading}
            />
          </label>
          <label className="login-label">
            Contraseña
            <input
              type="password"
              value={password}
              onChange={(e) => { setPassword(e.target.value); setError(""); }}
              placeholder="Mínimo 6 caracteres"
              className="login-input"
              autoComplete="new-password"
              disabled={loading}
            />
          </label>
          <button type="submit" className="login-button" disabled={loading}>
            {loading ? "Creando…" : "Crear cuenta"}
          </button>
        </form>
        <p className="login-footer">
          <Link to="/login">Volver al login</Link>
        </p>
      </div>
    </div>
  );
}
