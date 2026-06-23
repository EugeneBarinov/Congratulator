import { NavLink } from "react-router-dom";

export default function NavBar() {
  return (
    <header className="navbar">
      <NavLink to="/" className="brand">
        <span className="brand-mark" aria-hidden="true">
          <span className="brand-mark-dot" />
        </span>
        <span className="brand-name">Поздравлятор</span>
      </NavLink>

      <nav className="nav-links">
        <NavLink to="/" end className={({ isActive }) => (isActive ? "active" : "")}>
          Главная
        </NavLink>
        <NavLink to="/all" className={({ isActive }) => (isActive ? "active" : "")}>
          Все дни рождения
        </NavLink>
        <NavLink to="/add" className={({ isActive }) => (isActive ? "active" : "")}>
          Добавить
        </NavLink>
      </nav>
    </header>
  );
}
