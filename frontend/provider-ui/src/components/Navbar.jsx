import { Link, useNavigate } from "react-router-dom";
import "./Navbar.css";

export default function Navbar() {
  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.removeItem("token");
    navigate("/login");
  };

  return (
    <nav className="app-navbar">
      <div className="app-navbar-links">
        <Link to="/dashboard" className="app-navbar-link">
          Dashboard
        </Link>
        <Link to="/providers" className="app-navbar-link">
          Proveedores
        </Link>
      </div>
      <button type="button" onClick={handleLogout} className="app-navbar-logout">
        Logout
      </button>
    </nav>
  );
}
