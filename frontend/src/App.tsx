import { FormEvent, useEffect, useMemo, useState } from 'react';
import {
  claimItem,
  createFoundItem,
  createLostItem,
  getItem,
  getItems,
  Item,
  ItemPayload,
  ItemStatus,
  returnItem,
  verifyOwner
} from './api';

function getLocalDateTimeInputValue() {
  const now = new Date();
  const timezoneOffset = now.getTimezoneOffset() * 60_000;
  return new Date(now.getTime() - timezoneOffset).toISOString().slice(0, 16);
}

function createEmptyItemForm(): ItemPayload {
  return {
    name: '',
    description: '',
    category: '',
    location: '',
    faculty: '',
    reportDate: getLocalDateTimeInputValue(),
    photoUrl: '',
    characteristics: ''
  };
}

const statuses: ItemStatus[] = ['Reported', 'InCustody', 'Claimed', 'Returned'];

export default function App() {
  const [items, setItems] = useState<Item[]>([]);
  const [selectedItem, setSelectedItem] = useState<Item | null>(null);
  const [itemForm, setItemForm] = useState<ItemPayload>(createEmptyItemForm);
  const [mode, setMode] = useState<'lost' | 'found'>('lost');
  const [filters, setFilters] = useState({ status: '', location: '', faculty: '', date: '' });
  const [claimForm, setClaimForm] = useState({
    claimantName: '',
    claimantEmail: '',
    ownershipEvidence: ''
  });
  const [returnForm, setReturnForm] = useState({
    responsiblePerson: '',
    ownerConfirmation: true,
    notes: ''
  });
  const [message, setMessage] = useState('');

  const pendingCount = useMemo(
    () => items.filter((item) => item.status !== 'Returned').length,
    [items]
  );
  const returnedCount = items.length - pendingCount;

  async function loadItems() {
    const data = await getItems(filters);
    setItems(data);
    if (selectedItem) {
      const refreshed = data.find((item) => item.id === selectedItem.id);
      setSelectedItem(refreshed ?? null);
    }
  }

  useEffect(() => {
    loadItems().catch((error: Error) => setMessage(error.message));
  }, []);

  async function submitItem(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setMessage('');

    if (!itemForm.name || !itemForm.description || !itemForm.category || !itemForm.location || !itemForm.faculty || !itemForm.characteristics) {
      setMessage('Completa los campos obligatorios del objeto.');
      return;
    }

    const { photoUrl, ...requiredItemFields } = itemForm;
    const payload = {
      ...requiredItemFields,
      reportDate: new Date(itemForm.reportDate).toISOString(),
      ...(photoUrl?.trim() ? { photoUrl: photoUrl.trim() } : {})
    };
    try {
      const created = mode === 'lost' ? await createLostItem(payload) : await createFoundItem(payload);
      setItemForm(createEmptyItemForm());
      setSelectedItem(created);
      await loadItems();
      setMessage('Objeto registrado correctamente.');
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'No se pudo registrar el objeto.');
    }
  }

  async function submitClaim(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!selectedItem) return;
    if (!claimForm.claimantName || !claimForm.claimantEmail || !claimForm.ownershipEvidence) {
      setMessage('Completa los datos de reclamacion.');
      return;
    }

    await claimItem(selectedItem.id, claimForm);
    await refreshSelected(selectedItem.id);
    await loadItems();
    setClaimForm({ claimantName: '', claimantEmail: '', ownershipEvidence: '' });
    setMessage('Reclamacion registrada.');
  }

  async function verifyLatestClaim() {
    if (!selectedItem) return;
    const claim = selectedItem.claims.at(-1);
    if (!claim) {
      setMessage('No hay reclamaciones para verificar.');
      return;
    }

    await verifyOwner(selectedItem.id, claim.id);
    await refreshSelected(selectedItem.id);
    await loadItems();
    setMessage('Propietario verificado.');
  }

  async function submitReturn(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!selectedItem) return;
    const claim = selectedItem.claims.find((itemClaim) => itemClaim.isVerified);
    if (!claim) {
      setMessage('Primero verifica la propiedad.');
      return;
    }
    if (!returnForm.responsiblePerson) {
      setMessage('Indica la persona responsable de la devolucion.');
      return;
    }

    await returnItem(selectedItem.id, { claimId: claim.id, ...returnForm });
    await refreshSelected(selectedItem.id);
    await loadItems();
    setReturnForm({ responsiblePerson: '', ownerConfirmation: true, notes: '' });
    setMessage('Devolucion registrada.');
  }

  async function refreshSelected(id: string) {
    setSelectedItem(await getItem(id));
  }

  return (
    <main>
      <nav className="navbar">
        <h1>Objetos Perdidos Universidad</h1>
        <span>Pendientes: {pendingCount}</span>
        <span>Devueltos: {returnedCount}</span>
      </nav>

      {message && <div className="notice">{message}</div>}

      <section className="layout">
        <aside className="panel">
          <h2>Registrar objeto</h2>
          <div className="segmented">
            <button className={mode === 'lost' ? 'active' : ''} onClick={() => setMode('lost')}>Perdido</button>
            <button className={mode === 'found' ? 'active' : ''} onClick={() => setMode('found')}>Encontrado</button>
          </div>
          <form onSubmit={submitItem}>
            <input placeholder="Nombre" value={itemForm.name} onChange={(event) => setItemForm({ ...itemForm, name: event.target.value })} />
            <textarea placeholder="Descripcion" value={itemForm.description} onChange={(event) => setItemForm({ ...itemForm, description: event.target.value })} />
            <input placeholder="Categoria" value={itemForm.category} onChange={(event) => setItemForm({ ...itemForm, category: event.target.value })} />
            <input placeholder="Ubicacion" value={itemForm.location} onChange={(event) => setItemForm({ ...itemForm, location: event.target.value })} />
            <input placeholder="Facultad" value={itemForm.faculty} onChange={(event) => setItemForm({ ...itemForm, faculty: event.target.value })} />
            <input type="datetime-local" value={itemForm.reportDate} onChange={(event) => setItemForm({ ...itemForm, reportDate: event.target.value })} />
            <input placeholder="URL de foto opcional" value={itemForm.photoUrl} onChange={(event) => setItemForm({ ...itemForm, photoUrl: event.target.value })} />
            <textarea placeholder="Caracteristicas" value={itemForm.characteristics} onChange={(event) => setItemForm({ ...itemForm, characteristics: event.target.value })} />
            <button type="submit">Guardar</button>
          </form>
        </aside>

        <section className="panel list-panel">
          <h2>Listado</h2>
          <form className="filters" onSubmit={(event) => { event.preventDefault(); loadItems().catch((error: Error) => setMessage(error.message)); }}>
            <select value={filters.status} onChange={(event) => setFilters({ ...filters, status: event.target.value })}>
              <option value="">Todos los estados</option>
              {statuses.map((status) => <option key={status} value={status}>{status}</option>)}
            </select>
            <input placeholder="Ubicacion" value={filters.location} onChange={(event) => setFilters({ ...filters, location: event.target.value })} />
            <input placeholder="Facultad" value={filters.faculty} onChange={(event) => setFilters({ ...filters, faculty: event.target.value })} />
            <input type="date" value={filters.date} onChange={(event) => setFilters({ ...filters, date: event.target.value })} />
            <button type="submit">Filtrar</button>
          </form>

          <div className="items">
            {items.map((item) => (
              <button key={item.id} className="item-row" onClick={() => setSelectedItem(item)}>
                <span>
                  <strong>{item.name}</strong>
                  <small>{item.location} · {item.faculty}</small>
                </span>
                <span className={`badge ${item.status.toLowerCase()}`}>{item.status}</span>
              </button>
            ))}
          </div>
        </section>

        <aside className="panel detail">
          <h2>Detalle</h2>
          {selectedItem ? (
            <>
              <h3>{selectedItem.name}</h3>
              <p>{selectedItem.description}</p>
              <dl>
                <dt>Tipo</dt><dd>{selectedItem.itemType}</dd>
                <dt>Estado</dt><dd>{selectedItem.status}</dd>
                <dt>Categoria</dt><dd>{selectedItem.category}</dd>
                <dt>Caracteristicas</dt><dd>{selectedItem.characteristics}</dd>
              </dl>

              <h3>Reclamar</h3>
              <form onSubmit={submitClaim}>
                <input placeholder="Nombre reclamante" value={claimForm.claimantName} onChange={(event) => setClaimForm({ ...claimForm, claimantName: event.target.value })} />
                <input placeholder="Email" type="email" value={claimForm.claimantEmail} onChange={(event) => setClaimForm({ ...claimForm, claimantEmail: event.target.value })} />
                <textarea placeholder="Evidencia de propiedad" value={claimForm.ownershipEvidence} onChange={(event) => setClaimForm({ ...claimForm, ownershipEvidence: event.target.value })} />
                <button type="submit">Registrar reclamo</button>
              </form>

              <h3>Custodia</h3>
              <button onClick={verifyLatestClaim}>Verificar ultimo reclamo</button>
              <form onSubmit={submitReturn}>
                <input placeholder="Responsable" value={returnForm.responsiblePerson} onChange={(event) => setReturnForm({ ...returnForm, responsiblePerson: event.target.value })} />
                <label className="check">
                  <input type="checkbox" checked={returnForm.ownerConfirmation} onChange={(event) => setReturnForm({ ...returnForm, ownerConfirmation: event.target.checked })} />
                  Confirmacion del propietario
                </label>
                <textarea placeholder="Notas" value={returnForm.notes} onChange={(event) => setReturnForm({ ...returnForm, notes: event.target.value })} />
                <button type="submit">Registrar devolucion</button>
              </form>
            </>
          ) : (
            <p>Selecciona un objeto para ver su informacion.</p>
          )}
        </aside>
      </section>
    </main>
  );
}
