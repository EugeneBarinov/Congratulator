import { useEffect, useState } from "react";
import { peopleApi } from "../api/client.js";
import PersonCard from "../components/PersonCard.jsx";
import EmptyState from "../components/EmptyState.jsx";
import ConfirmDialog from "../components/ConfirmDialog.jsx";

export default function HomePage() {
  const [people, setPeople] = useState(null);
  const [error, setError] = useState(null);
  const [toDelete, setToDelete] = useState(null);

  const load = () => {
    setError(null);
    peopleApi
      .getTodayAndUpcoming()
      .then(setPeople)
      .catch(() => setError("Не удалось загрузить список. Проверьте, что API запущено."));
  };

  useEffect(load, []);

  const handleDelete = async () => {
    await peopleApi.remove(toDelete.id);
    setToDelete(null);
    load();
  };

  return (
    <>
      <div className="page-header">
        <h1>Сегодня и скоро</h1>
        <p>Дни рождения, которые наступают сегодня или в ближайшие дни.</p>
      </div>

      {error && <EmptyState title="Что-то пошло не так" description={error} />}

      {!error && people === null && <p style={{ color: "var(--color-ink-soft)" }}>Загрузка…</p>}

      {!error && people !== null && people.length === 0 && (
        <EmptyState
          title="Пока никого не ждём"
          description="Когда у кого-то из списка приблизится день рождения, он появится здесь."
          actionLabel="Добавить запись"
          actionTo="/add"
        />
      )}

      {!error && people !== null && people.length > 0 && (
        <ul className="ledger">
          {people.map((person) => (
            <PersonCard key={person.id} person={person} onDelete={setToDelete} />
          ))}
        </ul>
      )}

      <ConfirmDialog
        open={Boolean(toDelete)}
        title="Удалить запись?"
        description={toDelete ? `${toDelete.lastName} ${toDelete.firstName} будет удалён из списка без возможности восстановления.` : ""}
        onConfirm={handleDelete}
        onCancel={() => setToDelete(null)}
      />
    </>
  );
}
