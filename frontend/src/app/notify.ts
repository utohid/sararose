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

export function askContinueToDashboard(name: string, role: string, userType: string) {
  return Swal.fire({
    icon: 'success',
    title: 'Login successful',
    html: `<p style="margin:0 0 0.4rem;color:#ddd8cc">${name}</p>
           <p style="margin:0 0 0.85rem;letter-spacing:0.12em;text-transform:uppercase;font-size:0.75rem;color:#e0a106">${role} · ${userType}</p>
           <p style="margin:0;color:#b7b4a8">Continue to the dashboard?</p>`,
    confirmButtonText: 'Continue to dashboard',
    showCancelButton: true,
    cancelButtonText: 'Stay on login',
    allowOutsideClick: false,
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
