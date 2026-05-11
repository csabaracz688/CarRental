import React, { useEffect, useState } from "react";
import "../styles/EmployeeRentals.css";
import { useNavigate } from "react-router-dom";
import { useToast } from "../components/Toaster";

export default function EmployeeRentals() {
  const [rentals, setRentals] = useState([]);
  const [handoverDates, setHandoverDates] = useState({});
  const [statusFilter, setStatusFilter] = useState("");
  const token = localStorage.getItem("token");
  const navigate = useNavigate();
  const toast = useToast();

  const loadRentals = async () => {
    try {
      const res = await fetch("https://localhost:7077/api/rentals", {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      const text = await res.text();

      if (!res.ok) {
        console.error("Backend error:", res.status, text);
        return;
      }

      setRentals(text ? JSON.parse(text) : []);
    } catch (err) {
      console.error("Rentals loading error:", err);
    }
  };

  useEffect(() => {
    loadRentals();
  }, []);

  const approveRental = async (id) => {
    const res = await fetch(`https://localhost:7077/api/rentals/${id}/approve`, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    if (!res.ok) {
      toast("Approve failed!", { type: "error" });
      return;
    }

    loadRentals();
  };

  const rejectRental = async (id) => {
    const res = await fetch(`https://localhost:7077/api/rentals/${id}/reject`, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    if (!res.ok) {
      toast("Reject failed!", { type: "error" });
      return;
    }

    loadRentals();
  };

  const handOverRental = async (id) => {
    const rawDate = handoverDates[String(id)];

    if (!rawDate) {
      toast("Please provide handover date/time!", { type: "warning" });
      return;
    }

    const res = await fetch(`https://localhost:7077/api/rentals/${id}/handover`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({
        handedOverAt: rawDate,
      }),
    });

    if (!res.ok) {
      const text = await res.text();
      console.error("Handover error:", res.status, text);
      toast("Handover failed!", { type: "error" });
      return;
    }

    toast("Handover recorded!", { type: "success" });
    loadRentals();
  };

  const closeRental = async (id) => {
    const res = await fetch(`https://localhost:7077/api/rentals/${id}/close`, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    if (!res.ok) {
      const text = await res.text();
      console.error("Close error:", res.status, text);
      toast("Close failed!", { type: "error" });
      return;
    }

    toast("Rental closed!", { type: "success" });
    loadRentals();
  };

const openInvoice = async (id) => {
  const res = await fetch(`https://localhost:7077/api/rentals/${id}/invoice`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  if (!res.ok) {
    const text = await res.text();
    console.error("Invoice error:", res.status, text);
    toast("Unable to open invoice!", { type: "error" });
    return;
  }

  const html = await res.text();
  const blob = new Blob([html], { type: "text/html" });
  const url = URL.createObjectURL(blob);

  window.open(url, "_blank");
};
const filteredRentals = rentals.filter((rental) => {
  if (!statusFilter) return true;

  const status = (rental.statusText || rental.status || "")
    .toString()
    .toLowerCase();

  return status === statusFilter.toLowerCase();
});

  return (
    <div className="employee-rentals-page">
      <button className="back-btn" onClick={() => navigate("/employee")}>
        ← Back
      </button>

      <h1>Rentals</h1>
      <p>Review active and historical rentals.</p>

      <div className="rental-filters">
        <select
          value={statusFilter}
          onChange={(e) => setStatusFilter(e.target.value)}
        >
          <option value="">All statuses</option>
          <option value="Requested">Requested</option>
          <option value="Approved">Approved</option>
          <option value="Returned">Returned</option>
          <option value="Rejected">Rejected</option>
        </select>
      </div>

      <div className="employee-rentals-list">
        {filteredRentals.map((rental) => (
          <div key={rental.id} className="employee-rental-card">
            <img
              className="employee-rental-image"
              src={rental.imageUrl || "https://via.placeholder.com/300x180"}
              alt={`${rental.carBrand} ${rental.carModel}`}
            />

            <div className="employee-rental-info">
              <h2>
                {rental.carBrand} {rental.carModel}
              </h2>

              <p>
                <strong>License plate:</strong> {rental.licensePlate}
              </p>

              <p>
                <strong>Renter:</strong>{" "}
                {rental.customerName ||
                  rental.userName ||
                  rental.guestName ||
                  "Unknown"}
              </p>

              <p>
                <strong>Email:</strong>{" "}
                {rental.customerEmail ||
                  rental.userEmail ||
                  rental.guestEmail ||
                  "-"}
              </p>

              <p>
                <strong>Period:</strong>{" "}
                {new Date(rental.startDate).toLocaleDateString()} -{" "}
                {new Date(rental.endDate).toLocaleDateString()}
              </p>

              <p>
                <strong>Status:</strong> {rental.statusText || rental.status}
              </p>

              <p>
                <strong>Handover:</strong>{" "}
                {rental.handedOverAt
                  ? new Date(rental.handedOverAt).toLocaleString()
                  : "Not handed over yet"}
              </p>

              <p>
                <strong>Return / close:</strong>{" "}
                {rental.closedAt
                  ? new Date(rental.closedAt).toLocaleString()
                  : "Not closed yet"}
              </p>

              {(rental.statusText === "Requested" ||
                rental.status === 0 ||
                rental.status === "Requested") && (
                <div className="employee-rental-actions">
                  <button
                    className="approve-btn"
                    onClick={() => approveRental(rental.id)}
                  >
                    Accept
                  </button>

                  <button
                    className="reject-btn"
                    onClick={() => rejectRental(rental.id)}
                  >
                    Decline
                  </button>
                </div>
              )}

              {(rental.statusText === "Approved" ||
                rental.status === "Approved") &&
                !rental.handedOverAt && (
                  <div className="handover-box">
                    <input
                      type="datetime-local"
                      value={handoverDates[String(rental.id)] || ""}
                      onChange={(e) =>
                        setHandoverDates((prev) => ({
                          ...prev,
                          [String(rental.id)]: e.target.value,
                        }))
                      }
                    />

                    <button
                      className="handover-btn"
                      onClick={() => handOverRental(rental.id)}
                    >
                      Record handover
                    </button>
                  </div>
                )}

              {rental.handedOverAt && !rental.closedAt && (
                <button
                  className="close-btn"
                  onClick={() => closeRental(rental.id)}
                >
                  Close rental
                </button>
              )}

              {(rental.statusText === "Returned" ||
                rental.status === "Returned") && (
                <button
                  className="invoice-btn"
                  onClick={() => openInvoice(rental.id)}
                >
                  View invoice
                </button>
              )}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}