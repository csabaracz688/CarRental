import { Navigate, useLocation } from "react-router-dom";
import {
  clearSession,
  getHomeRouteByRole,
  getRole,
  getToken,
  isTokenExpired,
} from "../services/authService";

export default function ProtectedRoute({ children, role }) {
  const location = useLocation();
  const token = getToken();
  const userRole = getRole();

  if (!token || !userRole || isTokenExpired(token)) {
    clearSession();
    return <Navigate to="/login" state={{ from: location.pathname }} replace />;
  }

  if (role && userRole !== role) {
    return <Navigate to={getHomeRouteByRole(userRole)} replace />;
  }

  return children;
}