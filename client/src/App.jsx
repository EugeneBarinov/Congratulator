import { Routes, Route } from "react-router-dom";
import NavBar from "./components/NavBar.jsx";
import HomePage from "./pages/HomePage.jsx";
import AllPeoplePage from "./pages/AllPeoplePage.jsx";
import PersonFormPage from "./pages/PersonFormPage.jsx";

export default function App() {
  return (
    <div className="app-shell">
      <NavBar />
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/all" element={<AllPeoplePage />} />
        <Route path="/add" element={<PersonFormPage mode="create" />} />
        <Route path="/edit/:id" element={<PersonFormPage mode="edit" />} />
      </Routes>
    </div>
  );
}
