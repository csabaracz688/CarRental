import API from "./api";

export const getRentals = () =>
  API.get("/admin/rentals");

export const updateRentalStatus = (id, status) =>
  API.put(`/admin/rentals/${id}/status?status=${encodeURIComponent(status)}`);