import { useState, useEffect } from 'react';
import Header from './components/Header';
import Footer from './components/Footer';
import HomeView from './pages/Home/Index';
import ProductView from './pages/Product/Index';
import ProductDetailView from './pages/Product/Detail';
import CartView from './pages/Cart/Index';
import CheckoutView from './pages/Checkout/CheckoutView';
import PaymentView from './pages/Checkout/PaymentView';
import SearchView from './pages/SearchView';
import LoginView from './pages/Login/LoginView';
import UserInfoView from './pages/User/Index';
import AboutView from './pages/About/Index';
import PostDetailView from './pages/Blog/Detail';
import BlogView from './pages/Blog/Index';

const App = () => {
  const [currentView, setCurrentView] = useState({ name: 'home', params: {} });
  const [cart, setCart] = useState([]);

  useEffect(() => { window.scrollTo(0, 0); }, [currentView]);

  const navigate = (name, params = {}) => setCurrentView({ name, params });

  const addToCart = (product, size, qty) => {
    const existing = cart.find(item => item.id === product.id && item.size === size);
    if (existing) {
      setCart(cart.map(item => item.cartId === existing.cartId ? { ...item, qty: item.qty + qty } : item));
    } else {
      setCart([...cart, { ...product, size, qty, cartId: Date.now() + Math.random() }]);
    }
  };

  const updateQty = (cartId, newQty) => {
    if (newQty < 1) removeFromCart(cartId);
    else setCart(cart.map(item => item.cartId === cartId ? { ...item, qty: newQty } : item));
  };

  const removeFromCart = (cartId) => setCart(cart.filter(item => item.cartId !== cartId));
  const clearCart = () => setCart([]);

  const renderView = () => {
    switch (currentView.name) {
      case 'home': return <HomeView navigate={navigate} />;
      case 'products': return <ProductView navigate={navigate} />;
      case 'blog': return <BlogView navigate={navigate} />;
      case 'detail': return <ProductDetailView params={currentView.params} navigate={navigate} addToCart={addToCart} />;
      case 'cart': return <CartView cart={cart} updateQty={updateQty} removeFromCart={removeFromCart} navigate={navigate} />;
      case 'checkout': return <CheckoutView cart={cart} clearCart={clearCart} navigate={navigate} />;
      case 'payment': return <PaymentView navigate={navigate} clearCart={clearCart} />;
      case 'search': return <SearchView navigate={navigate} />;
      case 'login': return <LoginView navigate={navigate} />;
      case 'user': return <UserInfoView navigate={navigate} />;
      case 'about': return <AboutView />;
      case 'postDetail': return <PostDetailView id={currentView.params.id} navigate={navigate} />;
      default: return <HomeView navigate={navigate} />;
    }
  };

  return (
    <div>
      <Header currentView={currentView} navigate={navigate} cartCount={cart.reduce((sum, item) => sum + item.qty, 0)} />
      {renderView()}
      <Footer navigate={navigate} />
    </div>
  );
};

export default App;

