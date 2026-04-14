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
      const response = await axios.post(
        "http://localhost:5168/api/auth/login",
        {
          email,
          password,
        }
      );

      //token és role mentés
      localStorage.setItem("token", response.data.token);
      localStorage.setItem("role", response.data.role);

      //átirányítás role alapján
      if (response.data.role === "ADMIN") {
        navigate("/admin");
      } else if (response.data.role === "EMPLOYEE") {
        navigate("/employee");
      } else {
        navigate("/");
      }

      //ilyen backend választ vár: { "token": "abc123","role": "ADMIN"}

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