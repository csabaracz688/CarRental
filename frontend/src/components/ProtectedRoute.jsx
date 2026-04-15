import { Navigate } from "react-router-dom";

export default function ProtectedRoute({ children, role }) {
  const isDev = true; // 🔥 kapcsold ki productionben

  if (isDev) {
    return children;
  }

  const token = localStorage.getItem("token");
  const userRole = localStorage.getItem("role");

  if (!token || !userRole) {
    localStorage.removeItem("token");
    localStorage.removeItem("role");
    return <Navigate to="/login" />;
  }

  if (role && userRole !== role) {
    return <Navigate to="/" />;
  }

  return children;
}