import { useEffect, useState } from 'react';
import './App.css';
import { useUserStore } from './stores/user';
import { Login } from './components/login';

function App() {
    const [forecasts, setForecasts] = useState();

    useEffect(() => {
    }, []);

    const user = useUserStore((state) => state.user);
    
    return (
        <div className="App">
            {user == null ? <Login/> : 
            <div>
                <p>Welcome {user.name}</p>
            </div>}
        </div>
    )
}

export default App;