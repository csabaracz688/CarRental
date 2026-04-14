import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import api from "../services/api";
import "../styles/Login.css";

function Login() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      const response = await api.post("/auth/login", {
        email,
        password,
      });

      const { token, role } = response.data;

      //ilyen backend választ vár: { "token": "abc123","role": "ADMIN"}
      if (!token || typeof token !== "string" || !role || typeof role !== "string") {
        alert("Hibás bejelentkezési válasz. Kérjük próbálja újra!");
        return;
      }

      //token és role mentés
      localStorage.setItem("token", token);
      localStorage.setItem("role", role);

      //átirányítás role alapján
      if (role === "ADMIN") {
        navigate("/admin");
      } else if (role === "EMPLOYEE") {
        navigate("/employee");
      } else {
        navigate("/");
      }

    } catch (error) {
      console.error(error);
      alert("Hibás email vagy jelszó!");
    }
  };

  return (
    <div className="auth-container">
      <form className="auth-form" onSubmit={handleSubmit}>
        <h2>Login</h2>

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

        <button type="submit">Login</button>

        <p className="auth-link">
          Don't have an account? <Link to="/register">Register</Link>
        </p>
      </form>
    </div>
  );
}

export default Login;