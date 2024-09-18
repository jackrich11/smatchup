import * as React from 'react';
import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import Toolbar from '@mui/material/Toolbar';
import IconButton from '@mui/material/IconButton';
import Typography from '@mui/material/Typography';
import Menu from '@mui/material/Menu';
import MenuIcon from '@mui/icons-material/Menu';
import Container from '@mui/material/Container';
import Avatar from '@mui/material/Avatar';
import Button from '@mui/material/Button';
import MenuItem from '@mui/material/MenuItem';
import { Link, NavLink, useNavigate } from 'react-router-dom';
import { Tooltip, useTheme } from '@mui/material';
import { getEnvVar } from '../utils';

const pages = ['Matches', 'Stats', 'Settings'];

function Header({ currUser, isLoggedIn }) {
  const theme = useTheme()
  const nav = useNavigate()
  const [anchorElNav, setAnchorElNav] = React.useState(null);
  const [anchorElUser, setAnchorElUser] = React.useState(null);

  const handleOpenNavMenu = (event) => {
    setAnchorElNav(event.currentTarget);
  };
  const handleOpenUserMenu = (event) => {
    setAnchorElUser(event.currentTarget);
  };

  const handleCloseNavMenu = () => {
    setAnchorElNav(null);
  };

  const handleCloseUserMenu = () => {
    setAnchorElUser(null);
  };

  const menuNav = (page) => {
    nav("/" + page.toString())
  }

  return (
    <div className='header'>
        <AppBar sx={{ bgcolor: theme.palette.primary }}>
        <Container maxWidth="xl">
            <Toolbar disableGutters>
            <Typography
            variant="h6"
            noWrap
            sx={{
              mr: 2,
              display: { xs: 'none', md: 'flex' },
              fontFamily: 'monospace',
              fontWeight: 700,
              letterSpacing: '.3rem',
              color: 'inherit',
              textDecoration: 'none',
            }}
          >
            Smatchup
            </Typography>

            <Box sx={{ flexGrow: 1, display: { xs: 'flex', md: 'none' } }}>
                <IconButton
                size="large"
                aria-label="account of current user"
                aria-controls="menu-appbar"
                aria-haspopup="true"
                onClick={handleOpenNavMenu}
                color="inherit"
                >
                <MenuIcon />
                </IconButton>
                <Menu
                id="menu-appbar"
                anchorEl={anchorElNav}
                anchorOrigin={{
                    vertical: 'bottom',
                    horizontal: 'left',
                }}
                keepMounted
                transformOrigin={{
                    vertical: 'top',
                    horizontal: 'left',
                }}
                open={Boolean(anchorElNav)}
                onClose={handleCloseNavMenu}
                sx={{
                    display: { xs: 'block', md: 'none' },
                }}
                >
                {pages.map((page) => (
                    <MenuItem key={page} onClick={handleCloseNavMenu}>
                        <Typography onClick={() => menuNav(page.toLowerCase())} textAlign="center">{page}</Typography>
                    </MenuItem>
                ))}
                </Menu>
            </Box>

            <Typography
                variant="h5"
                noWrap
                sx={{
                mr: 2,
                display: { xs: 'flex', md: 'none' },
                flexGrow: 1,
                fontFamily: 'monospace',
                fontWeight: 700,
                letterSpacing: '.3rem',
                color: 'inherit',
                textDecoration: 'none',
                }}
            >
                Smatchup
            </Typography>

            <Box sx={{ flexGrow: 1, display: { xs: 'none', md: 'flex' } }}>
                {pages.map((page) => (
                <Button
                    key={page}
                    onClick={() => {
                        handleCloseNavMenu()
                        menuNav(page.toLowerCase())
                    }}
                    sx={{ my: 2, color: 'white', display: 'block' }}
                >
                    {page}
                </Button>
                ))}
            </Box>

            {!isLoggedIn && 
            <Button 
                variant='outline' 
                href={getEnvVar("DISCORD_URL")}
                sx={{
                    bgcolor: "#6320EE",
                    margin: 3,
                    ':hover': {
                        bgcolor: 'gray',
                    },
                }}
                >
                    
            Login with Discord!
            </Button>}

            {isLoggedIn && 
            <Box sx={{ flexGrow: 0}}>
                <Tooltip title={"logged in as " + currUser.Username}>
                    <IconButton onClick={handleOpenUserMenu} sx={{ p: 0 }}>
                        <Avatar alt={currUser.username} src={currUser.AvatarUrl} />
                    </IconButton>
                </Tooltip>
            </Box>}
            </Toolbar>
        </Container>
        </AppBar>
    </div>
  );
}
export default Header;