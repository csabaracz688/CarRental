import axios from "axios";

// Expected env var: VITE_API_BASE_URL (for example, "http://localhost:5000/api").
// Fall back to "/api" so fresh checkouts and CI builds still use a safe relative API path.
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || "/api";

const API = axios.create({
  baseURL: API_BASE_URL,
});

// TOKEN automatikus hozzáadás
API.interceptors.request.use((req) => {
  const token = localStorage.getItem("token");
  if (token) {
    req.headers.Authorization = `Bearer ${token}`;
  }
  return req;
});

export default API;