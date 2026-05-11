import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Grid,
  Typography,
  Divider,
  Chip,
  Box
} from '@mui/material'

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

interface TransactionDetailsDialogProps {
  open: boolean
  transaction: Transaction | null
  onClose: () => void
  onCreateDispute?: (transactionId: string) => void
}

export default function TransactionDetailsDialog({
  open,
  transaction,
  onClose,
  onCreateDispute
}: TransactionDetailsDialogProps) {
  if (!transaction) return null

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-ZA', {
      style: 'currency',
      currency: 'ZAR',
    }).format(amount)
  }

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
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
    <Dialog open={open} onClose={onClose} maxWidth="md" fullWidth>
      <DialogTitle>
        <Typography variant="h5" component="div">
          Transaction Details
        </Typography>
      </DialogTitle>
      <DialogContent dividers>
        <Grid container spacing={3}>
          <Grid item xs={12}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
              <Typography variant="h6" color="text.secondary">
                {transaction.merchantName}
              </Typography>
              <Chip
                label={transaction.status}
                color={getStatusColor(transaction.status)}
                size="medium"
              />
            </Box>
          </Grid>

          <Grid item xs={12}>
            <Divider />
          </Grid>

          <Grid item xs={12} sm={6}>
            <Typography variant="body2" color="text.secondary" gutterBottom>
              Transaction ID
            </Typography>
            <Typography variant="body1" fontFamily="monospace">
              {transaction.id}
            </Typography>
          </Grid>

          <Grid item xs={12} sm={6}>
            <Typography variant="body2" color="text.secondary" gutterBottom>
              Transaction Date
            </Typography>
            <Typography variant="body1">
              {formatDate(transaction.transactionDate)}
            </Typography>
          </Grid>

          <Grid item xs={12} sm={6}>
            <Typography variant="body2" color="text.secondary" gutterBottom>
              Amount
            </Typography>
            <Typography
              variant="h6"
              color={transaction.type === 'Debit' ? 'error' : 'success'}
            >
              {transaction.type === 'Debit' ? '-' : '+'}{formatCurrency(transaction.amount)}
            </Typography>
          </Grid>

          <Grid item xs={12} sm={6}>
            <Typography variant="body2" color="text.secondary" gutterBottom>
              Type
            </Typography>
            <Chip
              label={transaction.type}
              color={transaction.type === 'Debit' ? 'default' : 'success'}
            />
          </Grid>

          <Grid item xs={12} sm={6}>
            <Typography variant="body2" color="text.secondary" gutterBottom>
              Category
            </Typography>
            <Chip label={transaction.category} variant="outlined" />
          </Grid>

          <Grid item xs={12} sm={6}>
            <Typography variant="body2" color="text.secondary" gutterBottom>
              Currency
            </Typography>
            <Typography variant="body1">
              {transaction.currency}
            </Typography>
          </Grid>

          <Grid item xs={12}>
            <Typography variant="body2" color="text.secondary" gutterBottom>
              Description
            </Typography>
            <Typography variant="body1">
              {transaction.description}
            </Typography>
          </Grid>

          {transaction.isDisputed && (
            <Grid item xs={12}>
              <Chip
                label="This transaction has been disputed"
                color="error"
                variant="outlined"
                sx={{ width: '100%', justifyContent: 'center', py: 1 }}
              />
            </Grid>
          )}

          {transaction.createdDate && (
            <Grid item xs={12} sm={6}>
              <Typography variant="caption" color="text.secondary">
                Created: {formatDate(transaction.createdDate)}
              </Typography>
            </Grid>
          )}

          {transaction.updatedDate && (
            <Grid item xs={12} sm={6}>
              <Typography variant="caption" color="text.secondary">
                Updated: {formatDate(transaction.updatedDate)}
              </Typography>
            </Grid>
          )}
        </Grid>
      </DialogContent>
      <DialogActions>
        {transaction.canBeDisputed && onCreateDispute && (
          <Button
            onClick={() => {
              onCreateDispute(transaction.id)
              onClose()
            }}
            color="error"
            variant="contained"
          >
            Create Dispute
          </Button>
        )}
        <Button onClick={onClose} variant="outlined">
          Close
        </Button>
      </DialogActions>
    </Dialog>
  )
}
