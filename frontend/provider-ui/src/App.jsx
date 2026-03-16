import { Routes, Route, Navigate } from "react-router-dom";
import Layout from "./components/Layout.jsx";
import Login from "./pages/Login.jsx";
import Register from "./pages/Register.jsx";
import Dashboard from "./pages/Dashboard.jsx";
import Providers from "./pages/Providers.jsx";
import Users from "./pages/Users.jsx";

const TOKEN_KEY = "token";

function hasToken() {
  return !!localStorage.getItem(TOKEN_KEY);
}

function ProtectedRoute({ children }) {
  if (!hasToken()) {
    return <Navigate to="/login" replace />;
  }
  return children;
}

export default function App() {
  return (
    <Routes>
      <Route
        path="/login"
        element={
          hasToken() ? (
            <Navigate to="/dashboard" replace />
          ) : (
            <Login />
          )
        }
      />
      <Route
        path="/register"
        element={
          hasToken() ? (
            <Navigate to="/dashboard" replace />
          ) : (
            <Register />
          )
        }
      />
      <Route
        path="/"
        element={
          <ProtectedRoute>
            <Layout />
          </ProtectedRoute>
        }
      >
        <Route index element={<Navigate to="/dashboard" replace />} />
        <Route path="dashboard" element={<Dashboard />} />
        <Route path="providers" element={<Providers />} />
        <Route path="users" element={<Users />} />
      </Route>
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
