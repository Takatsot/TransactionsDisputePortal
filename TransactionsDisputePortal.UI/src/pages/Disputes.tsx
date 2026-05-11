import { 
  Typography, Paper, Box, CircularProgress, Alert,
  Table, TableBody, TableCell, TableContainer, TableHead, TableRow,
  Chip, IconButton, Tooltip, TextField, MenuItem, Grid, Card, CardContent,
  Button, Dialog, DialogTitle, DialogContent, DialogContentText, DialogActions
} from '@mui/material'
import { Visibility, CheckCircle, Cancel, HourglassEmpty, CancelOutlined } from '@mui/icons-material'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import axiosInstance from '../lib/axios'
import { useState } from 'react'
import DisputeDetailsDialog from '../components/DisputeDetailsDialog'

interface Transaction {
  id: string
  transactionDate: string
  amount: number
  currency: string
  merchantName: string
  description: string
  category: string
  type: string
}

interface Dispute {
  id: string
  transactionId: string
  transaction?: Transaction
  reason: string
  reasonDescription: string
  description: string
  status: string
  statusDescription: string
  createdDate: string
  updatedDate?: string
}

export default function Disputes() {
  const [statusFilter, setStatusFilter] = useState('All')
  const [searchTerm, setSearchTerm] = useState('')
  const [selectedDispute, setSelectedDispute] = useState<Dispute | null>(null)
  const [detailsDialogOpen, setDetailsDialogOpen] = useState(false)
  const [cancelDialogOpen, setCancelDialogOpen] = useState(false)
  const [disputeToCancel, setDisputeToCancel] = useState<Dispute | null>(null)
  const [cancelReason, setCancelReason] = useState('')
  const queryClient = useQueryClient()

  const { data, isLoading, error } = useQuery({
    queryKey: ['disputes'],
    queryFn: async () => {
      const response = await axiosInstance.get<{ data: Dispute[], totalCount: number }>('/api/disputes?pageNumber=1&pageSize=100')
      return response.data
    }
  })

  const cancelMutation = useMutation({
    mutationFn: async (disputeId: string) => {
      const response = await axiosInstance.put(`/api/disputes/${disputeId}/cancel`, {
        reason: cancelReason || 'Customer requested cancellation'
      })
      return response.data
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['disputes'] })
      queryClient.invalidateQueries({ queryKey: ['dashboard-disputes'] })
      setCancelDialogOpen(false)
      setDisputeToCancel(null)
      setCancelReason('')
    }
  })

  const handleOpenCancelDialog = (dispute: Dispute) => {
    setDisputeToCancel(dispute)
    setCancelDialogOpen(true)
  }

  const handleCancelDispute = () => {
    if (disputeToCancel) {
      cancelMutation.mutate(disputeToCancel.id)
    }
  }

  const handleViewDetails = (dispute: Dispute) => {
    setSelectedDispute(dispute)
    setDetailsDialogOpen(true)
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
        Error loading disputes: {error instanceof Error ? error.message : 'Unknown error'}
      </Alert>
    )
  }

  const disputes = data?.data || []
  
  const filteredDisputes = disputes.filter(d => {
    const matchesSearch = !searchTerm || 
      d.reasonDescription.toLowerCase().includes(searchTerm.toLowerCase()) ||
      d.description.toLowerCase().includes(searchTerm.toLowerCase()) ||
      d.transaction?.merchantName.toLowerCase().includes(searchTerm.toLowerCase()) ||
      d.transaction?.description.toLowerCase().includes(searchTerm.toLowerCase())
    const matchesStatus = statusFilter === 'All' || d.status === statusFilter
    return matchesSearch && matchesStatus
  })

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    })
  }

  const getStatusColor = (status: string): "default" | "warning" | "success" | "error" | "info" => {
    switch (status) {
      case 'Approved': return 'success'
      case 'Rejected': return 'error'
      case 'UnderReview': return 'warning'
      case 'Pending': return 'info'
      case 'Cancelled': return 'default'
      default: return 'default'
    }
  }

  const getStatusIcon = (status: string) => {
    switch (status) {
      case 'Approved': return <CheckCircle fontSize="small" />
      case 'Rejected': return <Cancel fontSize="small" />
      case 'UnderReview': return <HourglassEmpty fontSize="small" />
      case 'Pending': return <HourglassEmpty fontSize="small" />
      default: return undefined
    }
  }

  const getStatusLabel = (status: string) => {
    switch (status) {
      case 'UnderReview': return 'Under Review'
      default: return status
    }
  }

  // Calculate statistics
  const stats = {
    total: disputes.length,
    pending: disputes.filter(d => d.status === 'Pending').length,
    underReview: disputes.filter(d => d.status === 'UnderReview').length,
    approved: disputes.filter(d => d.status === 'Approved').length,
    rejected: disputes.filter(d => d.status === 'Rejected').length,
    cancelled: disputes.filter(d => d.status === 'Cancelled').length,
  }

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Disputes
      </Typography>
      <Typography variant="body2" color="text.secondary" paragraph>
        Manage and track your transaction disputes
      </Typography>

      {/* Statistics Cards */}
      <Grid container spacing={2} sx={{ mb: 3 }}>
        <Grid item xs={12} sm={6} md={2}>
          <Card>
            <CardContent>
              <Typography color="text.secondary" variant="body2">Total</Typography>
              <Typography variant="h4">{stats.total}</Typography>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} sm={6} md={2}>
          <Card>
            <CardContent>
              <Typography color="text.secondary" variant="body2">Pending</Typography>
              <Typography variant="h4" color="info.main">{stats.pending}</Typography>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} sm={6} md={2}>
          <Card>
            <CardContent>
              <Typography color="text.secondary" variant="body2">Under Review</Typography>
              <Typography variant="h4" color="warning.main">{stats.underReview}</Typography>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} sm={6} md={2}>
          <Card>
            <CardContent>
              <Typography color="text.secondary" variant="body2">Approved</Typography>
              <Typography variant="h4" color="success.main">{stats.approved}</Typography>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} sm={6} md={2}>
          <Card>
            <CardContent>
              <Typography color="text.secondary" variant="body2">Rejected</Typography>
              <Typography variant="h4" color="error.main">{stats.rejected}</Typography>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} sm={6} md={2}>
          <Card>
            <CardContent>
              <Typography color="text.secondary" variant="body2">Cancelled</Typography>
              <Typography variant="h4" color="text.secondary">{stats.cancelled}</Typography>
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      {/* Filters */}
      <Paper sx={{ p: 2, mb: 2 }}>
        <Grid container spacing={2}>
          <Grid item xs={12} md={6}>
            <TextField
              fullWidth
              label="Search"
              placeholder="Search by reason, merchant, or description"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              size="small"
            />
          </Grid>
          <Grid item xs={12} md={6}>
            <TextField
              fullWidth
              select
              label="Status"
              value={statusFilter}
              onChange={(e) => setStatusFilter(e.target.value)}
              size="small"
            >
              <MenuItem value="All">All Statuses</MenuItem>
              <MenuItem value="Pending">Pending</MenuItem>
              <MenuItem value="UnderReview">Under Review</MenuItem>
              <MenuItem value="Approved">Approved</MenuItem>
              <MenuItem value="Rejected">Rejected</MenuItem>
              <MenuItem value="Cancelled">Cancelled</MenuItem>
            </TextField>
          </Grid>
        </Grid>
      </Paper>

      {/* Disputes Table */}
      {filteredDisputes.length === 0 ? (
        <Paper sx={{ p: 3 }}>
          <Typography color="text.secondary">
            {disputes.length === 0 
              ? 'No disputes found. Create your first dispute from the transactions page.'
              : 'No disputes found matching your filters.'}
          </Typography>
        </Paper>
      ) : (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Created Date</TableCell>
                <TableCell>Transaction</TableCell>
                <TableCell>Reason</TableCell>
                <TableCell>Description</TableCell>
                <TableCell>Status</TableCell>
                <TableCell>Last Updated</TableCell>
                <TableCell align="center">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {filteredDisputes.map((dispute) => (
                <TableRow key={dispute.id} hover>
                  <TableCell>{formatDate(dispute.createdDate)}</TableCell>
                  <TableCell>
                    {dispute.transaction ? (
                      <Box>
                        <Typography variant="body2" fontWeight="medium">
                          {dispute.transaction.merchantName}
                        </Typography>
                        <Typography variant="caption" color="text.secondary">
                          {dispute.transaction.description}
                        </Typography>
                        <Typography variant="caption" display="block" color="text.secondary">
                          {dispute.transaction.currency} {dispute.transaction.amount.toFixed(2)}
                        </Typography>
                      </Box>
                    ) : (
                      <Typography variant="caption" color="text.secondary">
                        Transaction ID: {dispute.transactionId.substring(0, 8)}...
                      </Typography>
                    )}
                  </TableCell>
                  <TableCell>
                    <Chip 
                      label={dispute.reasonDescription}
                      size="small" 
                      variant="outlined"
                      color="primary"
                    />
                  </TableCell>
                  <TableCell>
                    <Typography variant="body2" color="text.secondary">
                      {dispute.description.length > 60 
                        ? dispute.description.substring(0, 60) + '...'
                        : dispute.description}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <Chip 
                      label={getStatusLabel(dispute.status)}
                      size="small" 
                      color={getStatusColor(dispute.status)}
                      icon={getStatusIcon(dispute.status)}
                    />
                  </TableCell>
                  <TableCell>
                    {dispute.updatedDate ? formatDate(dispute.updatedDate) : '-'}
                  </TableCell>
                  <TableCell align="center">
                    <Tooltip title="View Details">
                      <IconButton 
                        size="small" 
                        color="primary"
                        onClick={() => handleViewDetails(dispute)}
                      >
                        <Visibility fontSize="small" />
                      </IconButton>
                    </Tooltip>
                    {(dispute.status === 'Pending' || dispute.status === 'UnderReview') && (
                      <Tooltip title="Cancel Dispute">
                        <IconButton 
                          size="small" 
                          color="error"
                          onClick={() => handleOpenCancelDialog(dispute)}
                        >
                          <CancelOutlined fontSize="small" />
                        </IconButton>
                      </Tooltip>
                    )}
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      <DisputeDetailsDialog
        open={detailsDialogOpen}
        dispute={selectedDispute}
        onClose={() => setDetailsDialogOpen(false)}
      />

      {/* Cancel Dispute Confirmation Dialog */}
      <Dialog open={cancelDialogOpen} onClose={() => setCancelDialogOpen(false)}>
        <DialogTitle>Cancel Dispute</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Are you sure you want to cancel this dispute? This action cannot be undone.
          </DialogContentText>
          {disputeToCancel && (
            <Box sx={{ mt: 2, p: 2, bgcolor: 'grey.100', borderRadius: 1 }}>
              <Typography variant="body2" fontWeight="bold">
                Dispute Details:
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Reason: {disputeToCancel.reasonDescription}
              </Typography>
              <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
                Description: {disputeToCancel.description}
              </Typography>
              {disputeToCancel.transaction && (
                <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
                  Transaction: {disputeToCancel.transaction.merchantName} - {disputeToCancel.transaction.currency} {disputeToCancel.transaction.amount.toFixed(2)}
                </Typography>
              )}
            </Box>
          )}
          <TextField
            fullWidth
            label="Cancellation Reason (Optional)"
            placeholder="Provide a reason for cancelling..."
            multiline
            rows={3}
            value={cancelReason}
            onChange={(e) => setCancelReason(e.target.value)}
            sx={{ mt: 2 }}
          />
          {cancelMutation.error && (
            <Alert severity="error" sx={{ mt: 2 }}>
              {cancelMutation.error instanceof Error 
                ? cancelMutation.error.message 
                : 'Failed to cancel dispute'}
            </Alert>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setCancelDialogOpen(false)} disabled={cancelMutation.isPending}>
            Keep Dispute
          </Button>
          <Button 
            onClick={handleCancelDispute} 
            color="error" 
            variant="contained"
            disabled={cancelMutation.isPending}
          >
            {cancelMutation.isPending ? 'Cancelling...' : 'Cancel Dispute'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}