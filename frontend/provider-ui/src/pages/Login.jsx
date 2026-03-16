import { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { login } from "../services/api";
import "./Login.css";

export default function Login() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();
  const successMessage = location.state?.message;

  useEffect(() => {
    if (successMessage) setError("");
  }, [successMessage]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    if (!username.trim() || !password) {
      setError("Usuario y contraseña son requeridos.");
      return;
    }
    setLoading(true);
    try {
      const data = await login(username.trim(), password);
      localStorage.setItem("token", data.token);
      navigate("/dashboard");
    } catch (err) {
      const msg = err.response?.data?.message ?? err.response?.data ?? err.message ?? "Error al iniciar sesión.";
      setError(typeof msg === "string" ? msg : "Credenciales inválidas.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-page">
      <div className="login-card">
        <h1 className="login-title">Iniciar sesión</h1>
        {successMessage && <p className="login-success">{successMessage}</p>}
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
              autoFocus
              disabled={loading}
            />
          </label>
          <label className="login-label">
            Contraseña
            <input
              type="password"
              value={password}
              onChange={(e) => { setPassword(e.target.value); setError(""); }}
              placeholder="Contraseña"
              className="login-input"
              autoComplete="current-password"
              disabled={loading}
            />
          </label>
          <button type="submit" className="login-button" disabled={loading}>
            {loading ? "Entrando…" : "Login"}
          </button>
        </form>
      </div>
    </div>
  );
}
