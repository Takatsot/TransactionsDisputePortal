import { AppBar, Toolbar, Typography, Button, Container, Box, IconButton, Menu, MenuItem, Avatar, Divider } from '@mui/material'
import { Home, Receipt, Gavel, Logout } from '@mui/icons-material'
import { Link, useLocation, useNavigate } from 'react-router-dom'
import { ReactNode, useState } from 'react'
import { useAuth } from '../contexts/AuthContext'

interface LayoutProps {
  children: ReactNode
}

export default function Layout({ children }: LayoutProps) {
  const location = useLocation()
  const navigate = useNavigate()
  const { user, logout } = useAuth()
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null)

  const isActive = (path: string) => {
    return location.pathname === path
  }

  const handleMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget)
  }

  const handleMenuClose = () => {
    setAnchorEl(null)
  }

  const handleLogout = () => {
    logout()
    handleMenuClose()
    navigate('/login')
  }

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
      <AppBar position="static" elevation={2}>
        <Toolbar>
          <IconButton
            component={Link}
            to="/"
            color="inherit"
            edge="start"
            sx={{ mr: 2 }}
          >
            <Home />
          </IconButton>
          <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
            Transactions Dispute Portal
          </Typography>
          <Button
            component={Link}
            to="/"
            color="inherit"
            startIcon={<Home />}
            sx={{
              backgroundColor: isActive('/') ? 'rgba(255, 255, 255, 0.1)' : 'transparent',
              '&:hover': { backgroundColor: 'rgba(255, 255, 255, 0.15)' }
            }}
          >
            Dashboard
          </Button>
          <Button
            component={Link}
            to="/transactions"
            color="inherit"
            startIcon={<Receipt />}
            sx={{
              ml: 1,
              backgroundColor: isActive('/transactions') ? 'rgba(255, 255, 255, 0.1)' : 'transparent',
              '&:hover': { backgroundColor: 'rgba(255, 255, 255, 0.15)' }
            }}
          >
            Transactions
          </Button>
          <Button
            component={Link}
            to="/disputes"
            color="inherit"
            startIcon={<Gavel />}
            sx={{
              ml: 1,
              backgroundColor: isActive('/disputes') ? 'rgba(255, 255, 255, 0.1)' : 'transparent',
              '&:hover': { backgroundColor: 'rgba(255, 255, 255, 0.15)' }
            }}
          >
            Disputes
          </Button>
          
          <Box sx={{ ml: 2 }}>
            <IconButton
              onClick={handleMenuOpen}
              color="inherit"
              sx={{ p: 0.5 }}
            >
              <Avatar sx={{ width: 36, height: 36, bgcolor: 'secondary.main' }}>
                {user?.firstName?.charAt(0)}
              </Avatar>
            </IconButton>
            <Menu
              anchorEl={anchorEl}
              open={Boolean(anchorEl)}
              onClose={handleMenuClose}
              transformOrigin={{ horizontal: 'right', vertical: 'top' }}
              anchorOrigin={{ horizontal: 'right', vertical: 'bottom' }}
            >
              <Box sx={{ px: 2, py: 1, minWidth: 200 }}>
                <Typography variant="subtitle2" fontWeight="bold">
                  {user?.firstName} {user?.lastName}
                </Typography>
                <Typography variant="caption" color="text.secondary">
                  {user?.email}
                </Typography>
              </Box>
              <Divider />
              <MenuItem onClick={handleLogout}>
                <Logout fontSize="small" sx={{ mr: 1 }} />
                Logout
              </MenuItem>
            </Menu>
          </Box>
        </Toolbar>
      </AppBar>
      <Container maxWidth="xl" sx={{ mt: 4, mb: 4, flex: 1 }}>
        {children}
      </Container>
      <Box
        component="footer"
        sx={{
          py: 3,
          px: 2,
          mt: 'auto',
          backgroundColor: (theme) =>
            theme.palette.mode === 'light'
              ? theme.palette.grey[200]
              : theme.palette.grey[800],
        }}
      >
        <Container maxWidth="xl">
          <Typography variant="body2" color="text.secondary" align="center">
            © 2026 Transactions Dispute Portal - Digital Banking Solution
          </Typography>
        </Container>
      </Box>
    </Box>
  )
}
