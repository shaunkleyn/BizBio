/**
 * Menu Creation Wizard State Management
 * Controls whether the wizard is shown inline in the dashboard
 */

export const useMenuWizard = () => {
  const showWizard = useState('showMenuWizard', () => false)

  const openWizard = () => {
    showWizard.value = true
  }

  const closeWizard = () => {
    showWizard.value = false
  }

  return {
    showWizard,
    openWizard,
    closeWizard
  }
}
