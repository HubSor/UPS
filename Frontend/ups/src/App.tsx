import './style/App.css';
import Footer from './components/Footer';
import { Route, RouterProvider, createBrowserRouter, createRoutesFromElements } from 'react-router-dom';
import LoginPage from './pages/LoginPage';
import MainPage from './pages/MainPage';
import { AuthorizedPage } from './helpers/AuthorizedPage';
import { UnAuthorizedPage } from './helpers/UnAuthorizedPage';

export const Paths = {
    login: "/login",
    main: "/"
}

const router = createBrowserRouter(
    createRoutesFromElements(
        <Route>
            <Route path={Paths.main} element={<AuthorizedPage page={<MainPage/>} />} />
            <Route path={Paths.login} element={<UnAuthorizedPage page={<LoginPage/>} />} />
        </Route>
    )
)
export default function App() { 

    return <div className="app container">
        <RouterProvider router={router} />
        <Footer />
    </div>;
}
