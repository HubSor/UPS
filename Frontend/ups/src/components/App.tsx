import { useEffect } from 'react';
import '../style/App.css';
import axios from 'axios';

function App() {
  const url = process.env.REACT_APP_BACKEND_URL + "/test/get";

  useEffect(() => {
    axios.get(url).then(res => console.log(res));
  }, [url]);

  return <div className="App">
      <header className="App-header">
        <p>
          Backend url: {url}
        </p>
      </header>
    </div>
}

export default App;
