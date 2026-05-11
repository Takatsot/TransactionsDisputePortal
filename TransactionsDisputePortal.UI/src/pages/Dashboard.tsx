import { Grid, Card, CardContent, Typography, Box, Button, CircularProgress } from '@mui/material'
import { AccountBalance, Receipt, Gavel, TrendingDown, TrendingUp } from '@mui/icons-material'
import { Link } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import axiosInstance from '../lib/axios'

interface Transaction {
  id: string
  amount: number
  type: string
  status: string
}

interface Dispute {
  id: string
  status: string
}

export default function Dashboard() {
  const { data: transactionsData, isLoading: transactionsLoading } = useQuery({
    queryKey: ['dashboard-transactions'],
    queryFn: async () => {
      const response = await axiosInstance.get<{ data: Transaction[], totalCount: number }>('/api/transactions?pageNumber=1&pageSize=100')
      return response.data
    }
  })

  const { data: disputesData, isLoading: disputesLoading } = useQuery({
    queryKey: ['dashboard-disputes'],
    queryFn: async () => {
      const response = await axiosInstance.get<{ data: Dispute[], totalCount: number }>('/api/disputes?pageNumber=1&pageSize=100')
      return response.data
    }
  })

  const isLoading = transactionsLoading || disputesLoading

  // Calculate statistics
  const transactions = transactionsData?.data || []
  const disputes = disputesData?.data || []

  const stats = {
    totalTransactions: transactions.length,
    totalDebits: transactions.filter(t => t.type === 'Debit').length,
    totalCredits: transactions.filter(t => t.type === 'Credit').length,
    totalAmount: transactions.reduce((sum, t) => {
      return sum + (t.type === 'Debit' ? -t.amount : t.amount)
    }, 0),
    totalDisputes: disputes.length,
    pendingDisputes: disputes.filter(d => d.status === 'Pending').length,
    activeDisputes: disputes.filter(d => ['Pending', 'UnderReview'].includes(d.status)).length,
    resolvedDisputes: disputes.filter(d => ['Approved', 'Rejected'].includes(d.status)).length,
  }

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-ZA', {
      style: 'currency',
      currency: 'ZAR',
    }).format(Math.abs(amount))
  }

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Dashboard
      </Typography>
      <Typography variant="body1" color="text.secondary" paragraph>
        Welcome to the Transactions Dispute Portal. Here's your account overview.
      </Typography>

      {isLoading ? (
        <Box display="flex" justifyContent="center" alignItems="center" minHeight="300px">
          <CircularProgress />
        </Box>
      ) : (
        <>
          {/* Statistics Overview */}
          <Grid container spacing={3} sx={{ mb: 3 }}>
            <Grid item xs={12} md={3}>
              <Card sx={{ bgcolor: 'primary.main', color: 'white' }}>
                <CardContent>
                  <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                    <Box>
                      <Typography variant="body2" sx={{ opacity: 0.8 }}>
                        Total Transactions
                      </Typography>
                      <Typography variant="h4" sx={{ mt: 1 }}>
                        {stats.totalTransactions}
                      </Typography>
                    </Box>
                    <Receipt sx={{ fontSize: 48, opacity: 0.8 }} />
                  </Box>
                </CardContent>
              </Card>
            </Grid>

            <Grid item xs={12} md={3}>
              <Card sx={{ bgcolor: 'success.main', color: 'white' }}>
                <CardContent>
                  <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                    <Box>
                      <Typography variant="body2" sx={{ opacity: 0.8 }}>
                        Credits
                      </Typography>
                      <Typography variant="h4" sx={{ mt: 1 }}>
                        {stats.totalCredits}
                      </Typography>
                    </Box>
                    <TrendingUp sx={{ fontSize: 48, opacity: 0.8 }} />
                  </Box>
                </CardContent>
              </Card>
            </Grid>

            <Grid item xs={12} md={3}>
              <Card sx={{ bgcolor: 'error.main', color: 'white' }}>
                <CardContent>
                  <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                    <Box>
                      <Typography variant="body2" sx={{ opacity: 0.8 }}>
                        Debits
                      </Typography>
                      <Typography variant="h4" sx={{ mt: 1 }}>
                        {stats.totalDebits}
                      </Typography>
                    </Box>
                    <TrendingDown sx={{ fontSize: 48, opacity: 0.8 }} />
                  </Box>
                </CardContent>
              </Card>
            </Grid>

            <Grid item xs={12} md={3}>
              <Card sx={{ bgcolor: stats.totalAmount >= 0 ? 'success.dark' : 'error.dark', color: 'white' }}>
                <CardContent>
                  <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                    <Box>
                      <Typography variant="body2" sx={{ opacity: 0.8 }}>
                        Net Balance
                      </Typography>
                      <Typography variant="h5" sx={{ mt: 1 }}>
                        {stats.totalAmount >= 0 ? '+' : '-'}{formatCurrency(stats.totalAmount)}
                      </Typography>
                    </Box>
                    <AccountBalance sx={{ fontSize: 48, opacity: 0.8 }} />
                  </Box>
                </CardContent>
              </Card>
            </Grid>
          </Grid>

          {/* Action Cards */}
          <Grid container spacing={3} sx={{ mt: 2 }}>
            <Grid item xs={12} md={6}>
              <Card sx={{ height: '100%' }}>
                <CardContent>
                  <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                    <Receipt sx={{ fontSize: 40, color: 'primary.main', mr: 2 }} />
                    <Typography variant="h5">
                      Transactions
                    </Typography>
                  </Box>
                  <Typography variant="body2" color="text.secondary" paragraph>
                    View and manage all your financial transactions. Filter by date, amount, merchant, and status.
                  </Typography>
                  <Box sx={{ display: 'flex', gap: 2, mb: 2 }}>
                    <Box>
                      <Typography variant="h6" color="primary">
                        {stats.totalTransactions}
                      </Typography>
                      <Typography variant="caption" color="text.secondary">
                        Total Transactions
                      </Typography>
                    </Box>
                    <Box>
                      <Typography variant="h6" color="success.main">
                        {stats.totalCredits}
                      </Typography>
                      <Typography variant="caption" color="text.secondary">
                        Credits
                      </Typography>
                    </Box>
                    <Box>
                      <Typography variant="h6" color="error.main">
                        {stats.totalDebits}
                      </Typography>
                      <Typography variant="caption" color="text.secondary">
                        Debits
                      </Typography>
                    </Box>
                  </Box>
                  <Button 
                    component={Link} 
                    to="/transactions" 
                    variant="contained" 
                    fullWidth
                  >
                    View All Transactions
                  </Button>
                </CardContent>
              </Card>
            </Grid>

            <Grid item xs={12} md={6}>
              <Card sx={{ height: '100%' }}>
                <CardContent>
                  <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                    <Gavel sx={{ fontSize: 40, color: 'secondary.main', mr: 2 }} />
                    <Typography variant="h5">
                      Disputes
                    </Typography>
                  </Box>
                  <Typography variant="body2" color="text.secondary" paragraph>
                    Track and manage transaction disputes. Create new disputes, view status, and access dispute history.
                  </Typography>
                  <Box sx={{ display: 'flex', gap: 2, mb: 2 }}>
                    <Box>
                      <Typography variant="h6" color="secondary">
                        {stats.totalDisputes}
                      </Typography>
                      <Typography variant="caption" color="text.secondary">
                        Total Disputes
                      </Typography>
                    </Box>
                    <Box>
                      <Typography variant="h6" color="warning.main">
                        {stats.activeDisputes}
                      </Typography>
                      <Typography variant="caption" color="text.secondary">
                        Active
                      </Typography>
                    </Box>
                    <Box>
                      <Typography variant="h6" color="success.main">
                        {stats.resolvedDisputes}
                      </Typography>
                      <Typography variant="caption" color="text.secondary">
                        Resolved
                      </Typography>
                    </Box>
                  </Box>
                  <Button 
                    component={Link} 
                    to="/disputes" 
                    variant="contained" 
                    color="secondary" 
                    fullWidth
                  >
                    View All Disputes
                  </Button>
                </CardContent>
              </Card>
            </Grid>

            <Grid item xs={12}>
              <Card>
                <CardContent>
                  <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                    <AccountBalance sx={{ fontSize: 40, color: 'info.main', mr: 2 }} />
                    <Typography variant="h5">
                      About This Portal
                    </Typography>
                  </Box>
                  <Typography variant="body2" color="text.secondary" paragraph>
                    This portal provides a comprehensive solution for managing your financial transactions and disputes:
                  </Typography>
                  <Grid container spacing={2}>
                    <Grid item xs={12} md={6}>
                      <Typography variant="subtitle2" gutterBottom>Transaction Management:</Typography>
                      <ul style={{ margin: 0 }}>
                        <li><Typography variant="body2">Browse and search through your transaction history</Typography></li>
                        <li><Typography variant="body2">Filter by date, amount, merchant, category, and status</Typography></li>
                        <li><Typography variant="body2">View detailed transaction information</Typography></li>
                        <li><Typography variant="body2">Track disputed transactions</Typography></li>
                      </ul>
                    </Grid>
                    <Grid item xs={12} md={6}>
                      <Typography variant="subtitle2" gutterBottom>Dispute Management:</Typography>
                      <ul style={{ margin: 0 }}>
                        <li><Typography variant="body2">Create disputes for unauthorized or incorrect transactions</Typography></li>
                        <li><Typography variant="body2">Track the status and progress of your disputes</Typography></li>
                        <li><Typography variant="body2">View detailed history of all dispute actions</Typography></li>
                        <li><Typography variant="body2">Filter disputes by status</Typography></li>
                      </ul>
                    </Grid>
                  </Grid>
                </CardContent>
              </Card>
            </Grid>
          </Grid>
        </>
      )}
    </Box>
  )
}
