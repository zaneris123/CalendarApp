import { useEffect, useState } from 'react';
import './App.css';
import { useUserStore } from './stores/user';
import { Login } from './components/login';
import { Calendar } from './components/calendar';
import { Box } from '@mui/material';

function App() {

    useEffect(() => {
    }, []);

    const user = useUserStore((state) => state.user);
    
    return (
        <Box sx={{backgroundColor: '#b5b5b5', padding: '10px'}} className="App">
            {user == null ? <Login/> : 
            <div>
                <p>Welcome {user.name}</p>
                <button onClick={() => useUserStore.getState().clearUser()}>Logout</button>
                <Calendar/>
            </div>}
        </Box>
    )
}

export default App;