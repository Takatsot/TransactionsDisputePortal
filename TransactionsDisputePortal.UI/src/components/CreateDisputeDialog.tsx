import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  MenuItem,
  Alert,
  CircularProgress,
  Box,
  IconButton,
  Typography,
  List,
  ListItem,
  ListItemText,
  ListItemSecondaryAction,
  Paper
} from '@mui/material'
import { AttachFile, Delete } from '@mui/icons-material'
import { useState, useEffect, useRef } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import axiosInstance from '../lib/axios'

interface CreateDisputeDialogProps {
  open: boolean
  transactionId: string | null
  onClose: () => void
}

const disputeReasons = [
  { value: 1, label: 'Unauthorized Transaction' },
  { value: 2, label: 'Incorrect Amount' },
  { value: 3, label: 'Duplicate Charge' },
  { value: 4, label: 'Product Not Received' },
  { value: 5, label: 'Product Defective' },
  { value: 6, label: 'Service Not Provided' },
  { value: 7, label: 'Fraudulent Activity' },
  { value: 99, label: 'Other' }
]

export default function CreateDisputeDialog({
  open,
  transactionId,
  onClose
}: CreateDisputeDialogProps) {
  const [reason, setReason] = useState<number | ''>('')
  const [description, setDescription] = useState('')
  const [attachments, setAttachments] = useState<File[]>([])
  const fileInputRef = useRef<HTMLInputElement>(null)
  const queryClient = useQueryClient()

  useEffect(() => {
    if (open) {
      console.log('🔍 Dialog opened with transactionId:', transactionId)
    }
  }, [open, transactionId])

  const mutation = useMutation({
    mutationFn: async (data: { transactionId: string; reason: number; description: string; attachments: File[] }) => {
      const formData = new FormData()
      formData.append('transactionId', data.transactionId)
      formData.append('reason', data.reason.toString())
      formData.append('description', data.description)
      
      // Add attachments
      data.attachments.forEach((file) => {
        formData.append('attachments', file)
      })

      const response = await axiosInstance.post('/api/disputes', formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      })
      return response.data
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['transactions'] })
      queryClient.invalidateQueries({ queryKey: ['disputes'] })
      queryClient.invalidateQueries({ queryKey: ['dashboard-transactions'] })
      queryClient.invalidateQueries({ queryKey: ['dashboard-disputes'] })
      handleClose()
    }
  })

  const handleClose = () => {
    setReason('')
    setDescription('')
    setAttachments([])
    mutation.reset()
    onClose()
  }

  const handleFileSelect = (event: React.ChangeEvent<HTMLInputElement>) => {
    const files = Array.from(event.target.files || [])
    
    // Validate file sizes (max 10MB per file)
    const validFiles = files.filter(file => {
      if (file.size > 10 * 1024 * 1024) {
        alert(`File "${file.name}" exceeds maximum size of 10MB`)
        return false
      }
      return true
    })

    setAttachments(prev => [...prev, ...validFiles])
    
    // Reset file input
    if (fileInputRef.current) {
      fileInputRef.current.value = ''
    }
  }

  const handleRemoveFile = (index: number) => {
    setAttachments(prev => prev.filter((_, i) => i !== index))
  }

  const formatFileSize = (bytes: number) => {
    if (bytes < 1024) return bytes + ' B'
    if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + ' KB'
    return (bytes / (1024 * 1024)).toFixed(1) + ' MB'
  }

  const handleSubmit = () => {
    if (!transactionId || !reason || !description) return

    console.log('🔍 Creating dispute with data:', {
      transactionId,
      reason: Number(reason),
      description,
      attachments
    })

    mutation.mutate({
      transactionId,
      reason: Number(reason),
      description,
      attachments
    })
  }

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>Create Dispute</DialogTitle>
      <DialogContent dividers>
        {!transactionId && (
          <Alert severity="warning" sx={{ mb: 2 }}>
            No transaction selected. Please select a transaction first.
          </Alert>
        )}

        {mutation.isError && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {mutation.error instanceof Error
              ? mutation.error.message
              : 'Failed to create dispute. Please try again.'}
            {(mutation.error as any)?.response?.data?.detail && (
              <Box sx={{ mt: 1 }}>
                <strong>Details:</strong> {(mutation.error as any).response.data.detail}
              </Box>
            )}
            {(mutation.error as any)?.response?.data?.errors && (
              <Box component="ul" sx={{ mt: 1, pl: 2, mb: 0 }}>
                {Object.entries((mutation.error as any).response.data.errors).map(([field, messages]: [string, any]) => (
                  <li key={field}>
                    <strong>{field}:</strong> {Array.isArray(messages) ? messages.join(', ') : messages}
                  </li>
                ))}
              </Box>
            )}
          </Alert>
        )}

        {mutation.isSuccess && (
          <Alert severity="success" sx={{ mb: 2 }}>
            Dispute created successfully!
          </Alert>
        )}

        {/* File Upload Section */}
        <Box sx={{ mt: 3, mb: 2 }}>
          <Typography variant="subtitle2" gutterBottom>
            Attachments (Optional)
          </Typography>
          <Typography variant="caption" color="text.secondary" display="block" gutterBottom>
            Upload supporting documents such as receipts, statements, or screenshots. Max 10MB per file.
          </Typography>
          
          <input
            ref={fileInputRef}
            type="file"
            multiple
            accept="image/*,.pdf,.doc,.docx"
            onChange={handleFileSelect}
            style={{ display: 'none' }}
            disabled={mutation.isPending}
          />
          
          <Button
            variant="outlined"
            startIcon={<AttachFile />}
            onClick={() => fileInputRef.current?.click()}
            disabled={mutation.isPending}
            sx={{ mt: 1 }}
          >
            Add Files
          </Button>

          {attachments.length > 0 && (
            <Paper variant="outlined" sx={{ mt: 2, maxHeight: 200, overflow: 'auto' }}>
              <List dense>
                {attachments.map((file, index) => (
                  <ListItem key={index}>
                    <ListItemText
                      primary={file.name}
                      secondary={formatFileSize(file.size)}
                    />
                    <ListItemSecondaryAction>
                      <IconButton
                        edge="end"
                        onClick={() => handleRemoveFile(index)}
                        disabled={mutation.isPending}
                        size="small"
                      >
                        <Delete />
                      </IconButton>
                    </ListItemSecondaryAction>
                  </ListItem>
                ))}
              </List>
            </Paper>
          )}
        </Box>

        <TextField
          select
          fullWidth
          label="Reason for Dispute"
          value={reason}
          onChange={(e) => setReason(Number(e.target.value))}
          margin="normal"
          required
          disabled={mutation.isPending}
        >
          {disputeReasons.map((r) => (
            <MenuItem key={r.value} value={r.value}>
              {r.label}
            </MenuItem>
          ))}
        </TextField>

        <TextField
          fullWidth
          label="Description"
          multiline
          rows={4}
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          margin="normal"
          required
          disabled={mutation.isPending}
          placeholder="Please provide detailed information about why you're disputing this transaction..."
          helperText={`${description.length}/1000 characters (minimum 20 characters required)`}
          error={description.length > 0 && description.length < 20}
        />

        {transactionId && (
          <Box sx={{ mt: 2 }}>
            <Alert severity="info">
              Transaction ID: <strong>{transactionId}</strong>
            </Alert>
          </Box>
        )}
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose} disabled={mutation.isPending}>
          Cancel
        </Button>
        <Button
          onClick={handleSubmit}
          variant="contained"
          color="error"
          disabled={!reason || !description || description.length < 20 || mutation.isPending}
          startIcon={mutation.isPending ? <CircularProgress size={20} /> : null}
        >
          {mutation.isPending ? 'Submitting...' : 'Submit Dispute'}
        </Button>
      </DialogActions>
    </Dialog>
  )
}
