import { useState } from 'react'
import NewProject from './pages/NewProject'
import ExecutionView from './pages/ExecutionView'
import OutputView from './pages/OutputView'

type AppPage = 'new-project' | 'execution' | 'output'

export default function App() {
  const [page, setPage] = useState<AppPage>('new-project')
  const [projectId, setProjectId] = useState<string | null>(null)

  const handleProjectCreated = (id: string) => {
    setProjectId(id)
    setPage('execution')
  }

  const handleWorkflowCompleted = () => {
    setPage('output')
  }

  const handleNewProject = () => {
    setProjectId(null)
    setPage('new-project')
  }

  return (
    <div className="min-h-screen bg-[#0f1117]">
      {page === 'new-project' && (
        <NewProject onProjectCreated={handleProjectCreated} />
      )}
      {page === 'execution' && projectId && (
        <ExecutionView
          projectId={projectId}
          onCompleted={handleWorkflowCompleted}
        />
      )}
      {page === 'output' && projectId && (
        <OutputView projectId={projectId} onNewProject={handleNewProject} />
      )}
    </div>
  )
}
