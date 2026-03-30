import API from "./api";

export const getCars = () => API.get("/cars");

export const getAllCars = () => API.get("/admin/cars");

export const createCar = (formData) =>
  API.post("/admin/cars", formData);

export const updateCarStatus = (id, active) =>
  API.put(`/admin/cars/${id}/status?active=${active}`);