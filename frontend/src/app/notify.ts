import Swal from 'sweetalert2';

const theme = {
  confirmButtonColor: '#e0a106',
  cancelButtonColor: '#2a2d24',
  background: '#1a1c16',
  color: '#f3efe4'
};

export function notifySaved(title: string, text: string) {
  return Swal.fire({
    icon: 'success',
    title,
    text,
    confirmButtonText: 'OK',
    ...theme
  });
}

export function notifyError(text: string) {
  return Swal.fire({
    icon: 'error',
    title: 'Not saved',
    text,
    confirmButtonText: 'OK',
    ...theme
  });
}

export function confirmDelete(text: string) {
  return Swal.fire({
    icon: 'warning',
    title: 'Delete this item?',
    text,
    showCancelButton: true,
    confirmButtonText: 'Delete',
    cancelButtonText: 'Cancel',
    ...theme
  });
}
