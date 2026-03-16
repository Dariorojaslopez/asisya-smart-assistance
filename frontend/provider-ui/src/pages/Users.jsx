import { useState, useEffect } from "react";
import { getUsers, createUser, updateUser, deleteUser } from "../services/api";
import "./Users.css";

function formatDate(iso) {
  try {
    return new Date(iso).toLocaleString("es", {
      dateStyle: "short",
      timeStyle: "short",
    });
  } catch {
    return iso;
  }
}

export default function Users() {
  const [list, setList] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [modal, setModal] = useState(null); // null | "create" | { mode: "edit", id }
  const [form, setForm] = useState({ username: "", email: "", password: "", role: "User" });
  const [submitError, setSubmitError] = useState("");
  const [submitting, setSubmitting] = useState(false);

  const load = () => {
    setLoading(true);
    setError("");
    getUsers()
      .then(setList)
      .catch((err) => setError(err.response?.data?.message ?? err.message ?? "Error al cargar usuarios"))
      .finally(() => setLoading(false));
  };

  useEffect(() => load(), []);

  const openCreate = () => {
    setModal("create");
    setForm({ username: "", email: "", password: "", role: "User" });
    setSubmitError("");
  };

  const openEdit = (u) => {
    setModal({ mode: "edit", id: u.id });
    setForm({ username: u.username, email: u.email, password: "", role: u.role || "User" });
    setSubmitError("");
  };

  const closeModal = () => {
    setModal(null);
    setSubmitError("");
  };

  const handleCreate = async (e) => {
    e.preventDefault();
    setSubmitError("");
    if (!form.username.trim() || !form.email.trim() || !form.password) {
      setSubmitError("Usuario, email y contraseña son requeridos.");
      return;
    }
    if (form.password.length < 6) {
      setSubmitError("La contraseña debe tener al menos 6 caracteres.");
      return;
    }
    setSubmitting(true);
    try {
      await createUser({
        username: form.username.trim(),
        email: form.email.trim(),
        password: form.password,
        role: form.role || "User",
      });
      closeModal();
      load();
    } catch (err) {
      setSubmitError(err.response?.data?.message ?? err.message ?? "Error al crear usuario.");
    } finally {
      setSubmitting(false);
    }
  };

  const handleUpdate = async (e) => {
    e.preventDefault();
    setSubmitError("");
    if (!form.username.trim() || !form.email.trim()) {
      setSubmitError("Usuario y email son requeridos.");
      return;
    }
    setSubmitting(true);
    try {
      await updateUser(modal.id, {
        username: form.username.trim(),
        email: form.email.trim(),
        password: form.password || undefined,
        role: form.role || "User",
      });
      closeModal();
      load();
    } catch (err) {
      setSubmitError(err.response?.data?.message ?? err.message ?? "Error al actualizar usuario.");
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm("¿Eliminar este usuario?")) return;
    try {
      await deleteUser(id);
      load();
    } catch (err) {
      setError(err.response?.data?.message ?? err.message ?? "Error al eliminar.");
    }
  };

  const isEdit = modal && typeof modal === "object" && modal.mode === "edit";

  if (loading) return <p className="page-message">Cargando usuarios…</p>;

  return (
    <div className="users-page">
      <div className="users-header">
        <h1 className="page-title">Usuarios</h1>
        <button type="button" onClick={openCreate} className="btn btn-primary">
          Crear usuario
        </button>
      </div>
      {error && <p className="page-error">{error}</p>}
      <div className="table-wrap">
        <table className="data-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Username</th>
              <th>Email</th>
              <th>Role</th>
              <th>CreatedAt</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {list.length === 0 ? (
              <tr>
                <td colSpan={6} className="table-empty">
                  No hay usuarios. Crea el primero desde "Crear usuario" o desde el login (primera cuenta).
                </td>
              </tr>
            ) : (
              list.map((u) => (
                <tr key={u.id}>
                  <td>{u.id}</td>
                  <td>{u.username}</td>
                  <td>{u.email}</td>
                  <td>{u.role}</td>
                  <td>{formatDate(u.createdAt)}</td>
                  <td>
                    <button type="button" onClick={() => openEdit(u)} className="btn btn-sm btn-secondary">
                      Editar
                    </button>
                    <button type="button" onClick={() => handleDelete(u.id)} className="btn btn-sm btn-danger">
                      Eliminar
                    </button>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {modal && (
        <div className="modal-backdrop" onClick={closeModal}>
          <div className="modal-card" onClick={(e) => e.stopPropagation()}>
            <h2 className="modal-title">{isEdit ? "Editar usuario" : "Crear usuario"}</h2>
            {submitError && <p className="login-error">{submitError}</p>}
            <form onSubmit={isEdit ? handleUpdate : handleCreate} className="modal-form">
              <label className="login-label">
                Usuario
                <input
                  type="text"
                  value={form.username}
                  onChange={(e) => setForm((f) => ({ ...f, username: e.target.value }))}
                  className="login-input"
                  required
                  disabled={submitting}
                />
              </label>
              <label className="login-label">
                Email
                <input
                  type="email"
                  value={form.email}
                  onChange={(e) => setForm((f) => ({ ...f, email: e.target.value }))}
                  className="login-input"
                  required
                  disabled={submitting}
                />
              </label>
              <label className="login-label">
                Contraseña {isEdit && "(dejar vacío para no cambiar)"}
                <input
                  type="password"
                  value={form.password}
                  onChange={(e) => setForm((f) => ({ ...f, password: e.target.value }))}
                  className="login-input"
                  placeholder={isEdit ? "Opcional" : "Mínimo 6 caracteres"}
                  required={!isEdit}
                  minLength={isEdit ? 0 : 6}
                  disabled={submitting}
                />
              </label>
              <label className="login-label">
                Rol
                <select
                  value={form.role}
                  onChange={(e) => setForm((f) => ({ ...f, role: e.target.value }))}
                  className="login-input"
                  disabled={submitting}
                >
                  <option value="User">User</option>
                  <option value="Admin">Admin</option>
                </select>
              </label>
              <div className="modal-actions">
                <button type="button" onClick={closeModal} className="btn btn-secondary">
                  Cancelar
                </button>
                <button type="submit" className="btn btn-primary" disabled={submitting}>
                  {submitting ? "Guardando…" : isEdit ? "Guardar" : "Crear"}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
