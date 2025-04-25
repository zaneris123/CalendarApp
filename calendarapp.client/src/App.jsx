import { useEffect, useState } from 'react';
import './App.css';
import { useUserStore } from './stores/user';
import { Login } from './components/login';
import { Calendar } from './components/calendar';
import { Box } from '@mui/material';

function App() {
    const [forecasts, setForecasts] = useState();

    useEffect(() => {
    }, []);

    const user = useUserStore((state) => state.user);
    
    return (
        <Box sx={{backgroundColor: 'grey', padding: '10px'}} className="App">
            {user == null ? <Login/> : 
            <div>
                <p>Welcome {user.name}</p>
                <Calendar/>
            </div>}
        </Box>
    )
}

export default App;