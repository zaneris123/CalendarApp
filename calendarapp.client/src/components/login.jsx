import { use } from "react";
import Box from "@mui/material/Box";
import { useEffect, useState } from "react";
import { FormControl, InputLabel, MenuItem, Select, Button, TextField, Typography } from "@mui/material";
import { useUserStore } from "../stores/user";

export function Login() {
    const [loginUsers, setLoginUsers] = useState([]);
    const [selectedUserName, setSelectedUserName] = useState('');
    const [newUserName, setNewUserName] = useState('');

    const fetchUsers = () => {
        fetch("http://localhost:5142/users")
            .then((response) => {
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.json();
            })
            .then((data) => {
                console.log("Fetched users:", data);
                setLoginUsers(data);
            })
            .catch((error) => console.error("Error fetching login users:", error));
    };

    useEffect(() => {
        fetchUsers();
    }, []);

    const setUserInStore = useUserStore((state) => state.setUser);

    const handleLoginClick = () => {
        if (selectedUserName) {
            const userToLogin = loginUsers.find(user => user.name === selectedUserName);
            if (userToLogin) {
                setUserInStore(userToLogin);
            }
        }
    };

    const handleCreateUserClick = () => {
        if (newUserName.trim()) {
            fetch("http://localhost:5142/users", {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ name: newUserName.trim() }),
            })
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.json();
            })
            .then(createdUser => {
                console.log("User created:", createdUser);
                setNewUserName(''); // Clear input
                setUserInStore(createdUser); // Log in the newly created user
            })
            .catch(error => {
                console.error("Error creating user:", error);
            });
        }
    };

    return (
        <Box sx={{ minWidth: 300, display: 'flex', flexDirection: 'column', gap: 3 }}>
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
                <Typography variant="h6">Login</Typography>
                <FormControl fullWidth>
                    <InputLabel id="username-label">Select User</InputLabel>
                    <Select
                        labelId="username-label"
                        id="username"
                        value={selectedUserName}
                        label="Select User"
                        onChange={(event) => setSelectedUserName(event.target.value)}
                    >
                        {loginUsers.map((user) => (
                            <MenuItem key={user.id} value={user.name}>
                                {user.name}
                            </MenuItem>
                        ))}
                    </Select>
                </FormControl>
                <Button
                    variant="contained"
                    onClick={handleLoginClick}
                    disabled={!selectedUserName}
                >
                    Login
                </Button>
            </Box>

            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
                <Typography variant="h6">Or Create New User</Typography>
                <TextField
                    fullWidth
                    label="New Username"
                    variant="outlined"
                    value={newUserName}
                    onChange={(e) => setNewUserName(e.target.value)}
                />
                <Button
                    variant="contained"
                    color="secondary"
                    onClick={handleCreateUserClick}
                    disabled={!newUserName.trim()}
                >
                    Create User
                </Button>
            </Box>
        </Box>
    );
}
