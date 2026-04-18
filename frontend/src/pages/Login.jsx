import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import api from "../services/api";
import { getHomeRouteByRole, saveSession } from "../services/authService";
import "../styles/Login.css";

const SEEDED_USERS = [
  {
    label: "Admin",
    email: "admin@carrental.local",
    password: "Admin123!",
  },
  {
    label: "Officer",
    email: "officer@carrental.local",
    password: "Officer123!",
  },
  {
    label: "Customer",
    email: "customer@carrental.local",
    password: "Customer123!",
  },
];

function Login() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const navigate = useNavigate();

  const loginWithCredentials = async (nextEmail, nextPassword) => {
    setIsSubmitting(true);
    setError("");

    try {
      const response = await api.post("/auth/login", {
        email: nextEmail,
        password: nextPassword,
      });

      const { token, role, userId, userName } = response.data;

      if (!token || typeof token !== "string" || !role || typeof role !== "string") {
        setError("Hibás bejelentkezés. Kérem, próbálja újra.");
        return;
      }

      saveSession({ token, role, userId, userName });
      navigate(getHomeRouteByRole(role), { replace: true });
    } catch (err) {
      const responseMessage = err.response?.data?.message;
      setError(responseMessage || "Hibás bejelentkezés. Kérem, próbálja újra.");
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    await loginWithCredentials(email, password);
  };

  const handleQuickFill = (seededUser) => {
    setEmail(seededUser.email);
    setPassword(seededUser.password);
    setError("");
  };

  const handleQuickLogin = async (seededUser) => {
    setEmail(seededUser.email);
    setPassword(seededUser.password);
    await loginWithCredentials(seededUser.email, seededUser.password);
  };


  return (
    <div className="auth-container">
      <form className="auth-form" onSubmit={handleSubmit}>
        <h2>Login</h2>

        {error && <p className="auth-error">{error}</p>}

        <input
          type="email"
          placeholder="Email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />

        <input
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />

        <button type="submit" disabled={isSubmitting}>
          {isSubmitting ? "Logging in..." : "Login"}
        </button>

        <div className="seeded-users">
          <p className="seeded-users-title">Generated test users</p>
          {SEEDED_USERS.map((seededUser) => (
            <div key={seededUser.email} className="seeded-user-row">
              <div className="seeded-user-meta">
                <strong>{seededUser.label}</strong>
                <span>{seededUser.email}</span>
              </div>
              <div className="seeded-user-actions">
                <button
                  type="button"
                  className="seeded-fill-btn"
                  onClick={() => handleQuickFill(seededUser)}
                  disabled={isSubmitting}
                >
                  Fill
                </button>
                <button
                  type="button"
                  className="seeded-login-btn"
                  onClick={() => handleQuickLogin(seededUser)}
                  disabled={isSubmitting}
                >
                  Login now
                </button>
              </div>
            </div>
          ))}
        </div>

        <p className="auth-link">
          Nincs még fiókja? <Link to="/register">Regisztráció</Link>
        </p>
      </form>
    </div>
  );
}

export default Login;