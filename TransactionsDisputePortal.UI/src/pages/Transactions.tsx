import { 
  Typography, Paper, Box, CircularProgress, Alert, 
  Table, TableBody, TableCell, TableContainer, TableHead, TableRow,
  Chip, IconButton, Tooltip, TextField, MenuItem, Grid
} from '@mui/material'
import { Gavel, Visibility } from '@mui/icons-material'
import { useQuery } from '@tanstack/react-query'
import axiosInstance from '../lib/axios'
import { useState } from 'react'
import TransactionDetailsDialog from '../components/TransactionDetailsDialog'
import CreateDisputeDialog from '../components/CreateDisputeDialog'

interface Transaction {
  id: string
  transactionDate: string
  amount: number
  currency: string
  merchantName: string
  description: string
  category: string
  type: string
  status: string
  isDisputed: boolean
  canBeDisputed: boolean
  createdDate?: string
  updatedDate?: string
}

export default function Transactions() {
  const [searchTerm, setSearchTerm] = useState('')
  const [statusFilter, setStatusFilter] = useState('All')
  const [typeFilter, setTypeFilter] = useState('All')
  const [selectedTransaction, setSelectedTransaction] = useState<Transaction | null>(null)
  const [detailsDialogOpen, setDetailsDialogOpen] = useState(false)
  const [disputeDialogOpen, setDisputeDialogOpen] = useState(false)
  const [disputeTransactionId, setDisputeTransactionId] = useState<string | null>(null)

  const { data, isLoading, error } = useQuery({
    queryKey: ['transactions'],
    queryFn: async () => {
      const response = await axiosInstance.get<{ data: Transaction[], totalCount: number }>('/api/transactions?pageNumber=1&pageSize=100')
      return response.data
    }
  })

  const handleViewDetails = (transaction: Transaction) => {
    setSelectedTransaction(transaction)
    setDetailsDialogOpen(true)
  }

  const handleCreateDispute = (transactionId: string) => {
    setDisputeTransactionId(transactionId)
    setDisputeDialogOpen(true)
  }

  const handleCreateDisputeFromDetails = (transactionId: string) => {
    setDetailsDialogOpen(false)
    handleCreateDispute(transactionId)
  }

  if (isLoading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress />
      </Box>
    )
  }

  if (error) {
    return (
      <Alert severity="error">
        Error loading transactions: {error instanceof Error ? error.message : 'Unknown error'}
      </Alert>
    )
  }

  const transactions = data?.data || []
  
  const filteredTransactions = transactions.filter(t => {
    const matchesSearch = !searchTerm || 
      t.merchantName.toLowerCase().includes(searchTerm.toLowerCase()) ||
      t.description.toLowerCase().includes(searchTerm.toLowerCase()) ||
      t.category.toLowerCase().includes(searchTerm.toLowerCase())
    const matchesStatus = statusFilter === 'All' || t.status === statusFilter
    const matchesType = typeFilter === 'All' || t.type === typeFilter
    return matchesSearch && matchesStatus && matchesType
  })

  const formatAmount = (amount: number, type: string) => {
    const formatted = new Intl.NumberFormat('en-ZA', {
      style: 'currency',
      currency: 'ZAR',
    }).format(amount)
    return type === 'Debit' ? `-${formatted}` : `+${formatted}`
  }

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    })
  }

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Completed': return 'success'
      case 'Pending': return 'warning'
      case 'Disputed': return 'error'
      case 'Reversed': return 'info'
      default: return 'default'
    }
  }

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Transactions
      </Typography>
      <Typography variant="body2" color="text.secondary" paragraph>
        Total: {transactions.length} transactions
      </Typography>

      <Paper sx={{ p: 2, mb: 2 }}>
        <Grid container spacing={2}>
          <Grid item xs={12} md={4}>
            <TextField
              fullWidth
              label="Search"
              placeholder="Search by merchant, description, or category"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              size="small"
            />
          </Grid>
          <Grid item xs={12} md={4}>
            <TextField
              fullWidth
              select
              label="Status"
              value={statusFilter}
              onChange={(e) => setStatusFilter(e.target.value)}
              size="small"
            >
              <MenuItem value="All">All Statuses</MenuItem>
              <MenuItem value="Completed">Completed</MenuItem>
              <MenuItem value="Pending">Pending</MenuItem>
              <MenuItem value="Disputed">Disputed</MenuItem>
              <MenuItem value="Reversed">Reversed</MenuItem>
            </TextField>
          </Grid>
          <Grid item xs={12} md={4}>
            <TextField
              fullWidth
              select
              label="Type"
              value={typeFilter}
              onChange={(e) => setTypeFilter(e.target.value)}
              size="small"
            >
              <MenuItem value="All">All Types</MenuItem>
              <MenuItem value="Debit">Debit</MenuItem>
              <MenuItem value="Credit">Credit</MenuItem>
            </TextField>
          </Grid>
        </Grid>
      </Paper>

      {filteredTransactions.length === 0 ? (
        <Paper sx={{ p: 3 }}>
          <Typography color="text.secondary">
            No transactions found matching your filters.
          </Typography>
        </Paper>
      ) : (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Date</TableCell>
                <TableCell>Merchant</TableCell>
                <TableCell>Description</TableCell>
                <TableCell>Category</TableCell>
                <TableCell align="right">Amount</TableCell>
                <TableCell>Type</TableCell>
                <TableCell>Status</TableCell>
                <TableCell align="center">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {filteredTransactions.map((transaction) => (
                <TableRow key={transaction.id} hover>
                  <TableCell>{formatDate(transaction.transactionDate)}</TableCell>
                  <TableCell>
                    <Typography variant="body2" fontWeight="medium">
                      {transaction.merchantName}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <Typography variant="body2" color="text.secondary">
                      {transaction.description}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <Chip label={transaction.category} size="small" variant="outlined" />
                  </TableCell>
                  <TableCell align="right">
                    <Typography 
                      variant="body2" 
                      fontWeight="bold"
                      color={transaction.type === 'Debit' ? 'error' : 'success'}
                    >
                      {formatAmount(transaction.amount, transaction.type)}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <Chip 
                      label={transaction.type} 
                      size="small" 
                      color={transaction.type === 'Debit' ? 'default' : 'success'}
                    />
                  </TableCell>
                  <TableCell>
                    <Chip 
                      label={transaction.status} 
                      size="small" 
                      color={getStatusColor(transaction.status)}
                    />
                  </TableCell>
                  <TableCell align="center">
                    <Box sx={{ display: 'flex', gap: 1, justifyContent: 'center' }}>
                      <Tooltip title="View Details">
                        <IconButton 
                          size="small" 
                          color="primary"
                          onClick={() => handleViewDetails(transaction)}
                        >
                          <Visibility fontSize="small" />
                        </IconButton>
                      </Tooltip>
                      {transaction.canBeDisputed && (
                        <Tooltip title="Create Dispute">
                          <IconButton 
                            size="small" 
                            color="error"
                            onClick={() => handleCreateDispute(transaction.id)}
                          >
                            <Gavel fontSize="small" />
                          </IconButton>
                        </Tooltip>
                      )}
                      {transaction.isDisputed && (
                        <Chip 
                          label="Disputed" 
                          size="small" 
                          color="error" 
                          variant="outlined"
                        />
                      )}
                    </Box>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      <TransactionDetailsDialog
        open={detailsDialogOpen}
        transaction={selectedTransaction}
        onClose={() => setDetailsDialogOpen(false)}
        onCreateDispute={handleCreateDisputeFromDetails}
      />

      <CreateDisputeDialog
        open={disputeDialogOpen}
        transactionId={disputeTransactionId}
        onClose={() => setDisputeDialogOpen(false)}
      />
    </Box>
  )
}

