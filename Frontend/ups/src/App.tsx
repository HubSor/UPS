import './style/App.css';
import Header from './components/Header';
import Footer from './components/Footer';
import { Route, RouterProvider, createBrowserRouter, createRoutesFromElements } from 'react-router-dom';
import LoginPage from './pages/LoginPage';

const router = createBrowserRouter(
    createRoutesFromElements(
        <Route path='/' >
            <Route path='login' element={<LoginPage/>} />
        </Route>
    )
)
export default function App() { 

    return <div className="app container">
        <Header />
            <RouterProvider router={router} />
        <Footer />
    </div>;
}
