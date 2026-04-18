import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import api from "../services/api";
import { getHomeRouteByRole, saveSession } from "../services/authService";
import "../styles/Register.css";

function Register() {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    userName: "",
    email: "",
    password: "",
    firstName: "",
    lastName: "",
    address: "",
    phone: ""
  });
  const [error, setError] = useState("");

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");

    try {
      const response = await api.post("/auth/register", formData);
      const { token, role, userId, userName } = response.data;

      if (!token || !role) {
        setError("Hibas regisztracios valasz.");
        return;
      }

      saveSession({ token, role, userId, userName });
      navigate(getHomeRouteByRole(role), { replace: true });
    } catch (err) {
      const responseMessage = err.response?.data?.message;
      setError(responseMessage || "Sikertelen regisztracio.");
    }
  };


  return (
    <div className="auth-container">
      <form className="auth-form" onSubmit={handleSubmit}>
        <h2>Register</h2>

        {error && <p className="auth-error">{error}</p>}

        <input type="text" name="userName" placeholder="Username" onChange={handleChange} required />
        <input type="email" name="email" placeholder="Email" onChange={handleChange} required />
        <input type="password" name="password" placeholder="Password" onChange={handleChange} required />
        <input type="text" name="firstName" placeholder="First Name" onChange={handleChange} required/>
        <input type="text" name="lastName" placeholder="Last Name" onChange={handleChange} required />
        <input type="text" name="address" placeholder="Address" onChange={handleChange} required />
        <input type="text" name="phone" placeholder="Phone" onChange={handleChange} required />

        <button type="submit">Register</button>

        <p className="auth-link">
          Already registered? <Link to="/login">Login</Link>
        </p>
      </form>
    </div>
  );
}

export default Register;