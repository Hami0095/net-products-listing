const API_BASE = 'http://localhost:5000/api';

export type Product = {
  id: string;
  name: string;
  description: string;
  price: number;
  colour: string;
  createdAt: string;
};

export const login = async () => {
  const res = await fetch(`${API_BASE}/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
  });
  const data = await res.json();
  localStorage.setItem('token', data.token);
  return data.token;
};

export const getProducts = async (colour?: string): Promise<Product[]> => {
  const token = localStorage.getItem('token');
  const url = colour ? `${API_BASE}/products?colour=${colour}` : `${API_BASE}/products`;
  const res = await fetch(url, {
    headers: {
      'Authorization': `Bearer ${token}`
    }
  });
  if (res.status === 401) throw new Error('Unauthorized');
  return res.json();
};

export const createProduct = async (product: Omit<Product, 'id' | 'createdAt'>): Promise<Product> => {
  const token = localStorage.getItem('token');
  const res = await fetch(`${API_BASE}/products`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify(product)
  });
  if (res.status === 401) throw new Error('Unauthorized');
  return res.json();
};
