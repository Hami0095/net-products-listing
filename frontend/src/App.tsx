import { useState, useEffect } from 'react';
import { Product, getProducts, createProduct, login } from './api';

function App() {
  const [products, setProducts] = useState<Product[]>([]);
  const [isLoggedIn, setIsLoggedIn] = useState(!!localStorage.getItem('token'));
  const [filter, setFilter] = useState('');
  const [loading, setLoading] = useState(false);
  const [newProduct, setNewProduct] = useState({
    name: '',
    description: '',
    price: 0,
    colour: ''
  });

  const fetchData = async (colourFilter?: string) => {
    try {
      setLoading(true);
      const data = await getProducts(colourFilter);
      setProducts(data);
    } catch (err) {
      if (err instanceof Error && err.message === 'Unauthorized') {
        setIsLoggedIn(false);
      }
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (isLoggedIn) {
      fetchData();
    }
  }, [isLoggedIn]);

  const handleLogin = async () => {
    await login();
    setIsLoggedIn(true);
  };

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    await createProduct(newProduct);
    setNewProduct({ name: '', description: '', price: 0, colour: '' });
    fetchData(filter);
  };

  if (!isLoggedIn) {
    return (
      <div className="container">
        <div className="card" style={{ maxWidth: '400px', margin: '100px auto', textAlign: 'center' }}>
          <h1>Welcome</h1>
          <p style={{ marginBottom: '20px', color: 'var(--text-muted)' }}>Sign in to manage products</p>
          <button onClick={handleLogin} style={{ width: '100%' }}>Login with Demo Account</button>
        </div>
      </div>
    );
  }

  return (
    <div className="container">
      <h1>Product Inventory</h1>

      <div className="card">
        <h3>Create New Product</h3>
        <form onSubmit={handleCreate} style={{ marginTop: '20px' }}>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '16px' }}>
            <input 
              placeholder="Product Name" 
              value={newProduct.name} 
              onChange={e => setNewProduct({...newProduct, name: e.target.value})} 
              required
            />
            <input 
              placeholder="Colour" 
              value={newProduct.colour} 
              onChange={e => setNewProduct({...newProduct, colour: e.target.value})} 
              required
            />
          </div>
          <input 
            placeholder="Description" 
            value={newProduct.description} 
            onChange={e => setNewProduct({...newProduct, description: e.target.value})} 
            required
          />
          <input 
            type="number" 
            placeholder="Price" 
            value={newProduct.price} 
            onChange={e => setNewProduct({...newProduct, price: parseFloat(e.target.value)})} 
            required
          />
          <button type="submit">Add Product</button>
        </form>
      </div>

      <div className="filters">
        <span>Filter by Colour:</span>
        <select 
          value={filter} 
          onChange={e => {
            setFilter(e.target.value);
            fetchData(e.target.value);
          }}
          style={{ width: 'auto', marginBottom: 0 }}
        >
          <option value="">All Colours</option>
          <option value="Red">Red</option>
          <option value="Blue">Blue</option>
          <option value="Green">Green</option>
          <option value="Black">Black</option>
          <option value="White">White</option>
        </select>
        {loading && <span style={{ marginLeft: '10px' }}>Loading...</span>}
      </div>

      <div className="grid">
        {products.map(p => (
          <div key={p.id} className="card product-card">
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
              <h3>{p.name}</h3>
              <span className="badge badge-colour">{p.colour}</span>
            </div>
            <p>{p.description}</p>
            <div style={{ fontWeight: 'bold', fontSize: '1.2rem', color: '#818cf8' }}>
              ${p.price.toFixed(2)}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}

export default App;
