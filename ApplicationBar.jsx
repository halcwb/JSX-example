import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import Toolbar from '@mui/material/Toolbar';
import IconButton from '@mui/material/IconButton';
import MenuIcon from '@mui/icons-material/Menu';

export function ApplicationBar() {
    return (
        <Box sx={{flexGrow: 1}}>
            <AppBar position="static">
            <Toolbar>
                <IconButton
                    size="large"
                    edge="start"
                    color="inherit"
                    aria-label="menu"
                    sx={{mr: 2}}>
                <MenuIcon />
                </IconButton>
            </Toolbar>
            </AppBar>
        </Box>
    );
}
