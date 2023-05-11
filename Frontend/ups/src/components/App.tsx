import { useState } from 'react';
import '../style/App.css';
import { AddProductForm } from './AddProductForm';
import { Products } from './Products';

export default function App() {
    const [refresh, setRefresh] = useState(true);

    return (
        <div className="App">
            <header className="App-header">
                <AddProductForm setRefresh={setRefresh} />
                <Products refresh={refresh} setRefresh={setRefresh} />
            </header>
        </div>
    );
}
