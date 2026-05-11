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
import { CheckCircle, Cancel, HourglassEmpty } from '@mui/icons-material'

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

interface DisputeDetailsDialogProps {
  open: boolean
  dispute: Dispute | null
  onClose: () => void
}

export default function DisputeDetailsDialog({
  open,
  dispute,
  onClose
}: DisputeDetailsDialogProps) {
  if (!dispute) return null

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
      case 'Approved': return <CheckCircle />
      case 'Rejected': return <Cancel />
      case 'UnderReview': return <HourglassEmpty />
      case 'Pending': return <HourglassEmpty />
      default: return undefined
    }
  }

  const getStatusLabel = (status: string) => {
    switch (status) {
      case 'UnderReview': return 'Under Review'
      default: return status
    }
  }

  const getStatusMessage = (status: string) => {
    switch (status) {
      case 'Pending':
        return 'Your dispute has been received and is waiting to be reviewed.'
      case 'UnderReview':
        return 'Your dispute is currently being reviewed by our team. This typically takes 3-5 business days.'
      case 'Approved':
        return 'Your dispute has been approved. The transaction amount will be refunded to your account within 5-7 business days.'
      case 'Rejected':
        return 'Your dispute has been rejected. If you have additional information, please contact customer support.'
      case 'Cancelled':
        return 'This dispute has been cancelled.'
      default:
        return ''
    }
  }

  return (
    <Dialog open={open} onClose={onClose} maxWidth="md" fullWidth>
      <DialogTitle>
        <Typography variant="h5" component="div">
          Dispute Details
        </Typography>
      </DialogTitle>
      <DialogContent dividers>
        <Grid container spacing={3}>
          <Grid item xs={12}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
              <Typography variant="h6" color="text.secondary">
                {dispute.reasonDescription}
              </Typography>
              <Chip
                label={getStatusLabel(dispute.status)}
                color={getStatusColor(dispute.status)}
                icon={getStatusIcon(dispute.status)}
                size="medium"
              />
            </Box>
          </Grid>

          <Grid item xs={12}>
            <Divider />
          </Grid>

          <Grid item xs={12}>
            <Box
              sx={{
                p: 2,
                bgcolor: 'background.paper',
                border: 1,
                borderColor: 'divider',
                borderRadius: 1
              }}
            >
              <Typography variant="body2" color="info.main">
                {getStatusMessage(dispute.status)}
              </Typography>
            </Box>
          </Grid>

          {dispute.transaction && (
            <>
              <Grid item xs={12}>
                <Typography variant="h6" gutterBottom>
                  Transaction Details
                </Typography>
              </Grid>

              <Grid item xs={12}>
                <Box
                  sx={{
                    p: 2,
                    bgcolor: 'primary.50',
                    borderRadius: 1,
                    border: 1,
                    borderColor: 'primary.200'
                  }}
                >
                  <Grid container spacing={2}>
                    <Grid item xs={12}>
                      <Typography variant="body2" color="text.secondary">
                        Merchant
                      </Typography>
                      <Typography variant="h6">
                        {dispute.transaction.merchantName}
                      </Typography>
                    </Grid>
                    <Grid item xs={12}>
                      <Typography variant="body2" color="text.secondary">
                        Transaction Description
                      </Typography>
                      <Typography variant="body1">
                        {dispute.transaction.description}
                      </Typography>
                    </Grid>
                    <Grid item xs={6}>
                      <Typography variant="body2" color="text.secondary">
                        Amount
                      </Typography>
                      <Typography variant="h6" color="primary">
                        {dispute.transaction.currency} {dispute.transaction.amount.toFixed(2)}
                      </Typography>
                    </Grid>
                    <Grid item xs={6}>
                      <Typography variant="body2" color="text.secondary">
                        Category
                      </Typography>
                      <Typography variant="body1">
                        {dispute.transaction.category}
                      </Typography>
                    </Grid>
                    <Grid item xs={12}>
                      <Typography variant="body2" color="text.secondary">
                        Transaction Date
                      </Typography>
                      <Typography variant="body1">
                        {formatDate(dispute.transaction.transactionDate)}
                      </Typography>
                    </Grid>
                  </Grid>
                </Box>
              </Grid>
            </>
          )}

          <Grid item xs={12}>
            <Divider />
          </Grid>

          <Grid item xs={12} sm={6}>
            <Typography variant="body2" color="text.secondary" gutterBottom>
              Dispute ID
            </Typography>
            <Typography variant="body1" fontFamily="monospace" fontSize="0.85rem">
              {dispute.id}
            </Typography>
          </Grid>

          <Grid item xs={12} sm={6}>
            <Typography variant="body2" color="text.secondary" gutterBottom>
              Transaction ID
            </Typography>
            <Typography variant="body1" fontFamily="monospace" fontSize="0.85rem">
              {dispute.transactionId}
            </Typography>
          </Grid>

          <Grid item xs={12} sm={6}>
            <Typography variant="body2" color="text.secondary" gutterBottom>
              Created Date
            </Typography>
            <Typography variant="body1">
              {formatDate(dispute.createdDate)}
            </Typography>
          </Grid>

          {dispute.updatedDate && (
            <Grid item xs={12} sm={6}>
              <Typography variant="body2" color="text.secondary" gutterBottom>
                Last Updated
              </Typography>
              <Typography variant="body1">
                {formatDate(dispute.updatedDate)}
              </Typography>
            </Grid>
          )}

          <Grid item xs={12}>
            <Typography variant="body2" color="text.secondary" gutterBottom>
              Dispute Reason
            </Typography>
            <Typography variant="body1" fontWeight="medium">
              {dispute.reasonDescription}
            </Typography>
          </Grid>

          <Grid item xs={12}>
            <Typography variant="body2" color="text.secondary" gutterBottom>
              Description
            </Typography>
            <Box
              sx={{
                p: 2,
                bgcolor: 'grey.50',
                borderRadius: 1,
                border: 1,
                borderColor: 'grey.200'
              }}
            >
              <Typography variant="body1" sx={{ whiteSpace: 'pre-wrap' }}>
                {dispute.description}
              </Typography>
            </Box>
          </Grid>
        </Grid>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} variant="outlined">
          Close
        </Button>
      </DialogActions>
    </Dialog>
  )
}
