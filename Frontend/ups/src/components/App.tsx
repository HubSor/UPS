import { useEffect } from 'react';
import '../style/App.css';

function App() {
  useEffect(() => {

  }, []);

  return <div className="App">
      <header className="App-header">
        <p>
          Backend url: {process.env.REACT_APP_BACKEND_URL ?? "brak"}
        </p>
      </header>
    </div>
}

export default App;
