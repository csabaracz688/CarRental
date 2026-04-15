function EmployeeRentals() {
  const rentals = [
    { id: 1, car: "BMW X5", user: "János", days: 3 },
    { id: 2, car: "Audi A4", user: "Anna", days: 5 },
  ];

  return (
    <div>
      <h1>Bérlések</h1>

      <ul>
        {rentals.map((rental) => (
          <li key={rental.id}>
            {rental.car} - {rental.user} ({rental.days} nap)
          </li>
        ))}
      </ul>
    </div>
  );
}

export default EmployeeRentals;