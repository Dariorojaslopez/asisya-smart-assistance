import { NavLink, useNavigate } from "react-router-dom";
import "./Sidebar.css";

export default function Sidebar() {
  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.removeItem("token");
    navigate("/login");
  };

  return (
    <aside className="sidebar">
      <div className="sidebar-header">
        <h2 className="sidebar-title">Provider Optimizer</h2>
      </div>
      <nav className="sidebar-nav">
        <NavLink to="/dashboard" className={({ isActive }) => "sidebar-link" + (isActive ? " active" : "")}>
          Dashboard
        </NavLink>
        <NavLink to="/providers" className={({ isActive }) => "sidebar-link" + (isActive ? " active" : "")}>
          Proveedores
        </NavLink>
        <NavLink to="/users" className={({ isActive }) => "sidebar-link" + (isActive ? " active" : "")}>
          Usuarios
        </NavLink>
      </nav>
      <div className="sidebar-footer">
        <button type="button" onClick={handleLogout} className="sidebar-logout">
          Logout
        </button>
      </div>
    </aside>
  );
}
