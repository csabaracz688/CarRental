function EmployeeRentals() {
  const rentals = [
    {
      id: 1,
      car: "BMW X5",
      user: "János",
      days: 3,
      status: "Requested",
      startDate: "2026-04-01",
    },
    {
      id: 2,
      car: "Audi A4",
      user: "Anna",
      days: 5,
      status: "Approved",
      startDate: "2026-03-28",
    },
    {
      id: 3,
      car: "Tesla Model 3",
      user: "Péter",
      days: 2,
      status: "Handed",
      startDate: "2026-04-10",
    },
    {
      id: 4,
      car: "Skoda Octavia",
      user: "Eszter",
      days: 7,
      status: "Returned",
      startDate: "2026-03-18",
    },
  ];

  return (
    <div>
      <h1>Bérlések</h1>
      <p>Aktív és korábbi bérlések áttekintése.</p>

      <ul>
        {rentals.map((rental) => (
          <li key={rental.id}>
            {rental.car} - {rental.user} ({rental.days} nap, {rental.status}, kezdés: {rental.startDate})
          </li>
        ))}
      </ul>
    </div>
  );
}

export default EmployeeRentals;