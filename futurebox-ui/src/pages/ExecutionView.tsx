import { useEffect, useState } from 'react'
import { HubConnectionBuilder, HubConnection, LogLevel } from '@microsoft/signalr'

interface Props {
  projectId: string
  onCompleted: () => void
}

interface PipelineStep {
  name: string
  label: string
  state: 'waiting' | 'running' | 'done' | 'failed'
  percent: number
}

interface LogEntry {
  timestamp: string
  agentName: string
  message: string
}

const PIPELINE_STEPS: Omit<PipelineStep, 'state' | 'percent'>[] = [
  { name: 'ResearchAgent', label: 'Research' },
  { name: 'ScriptAgent', label: 'Script' },
  { name: 'NarrationAgent', label: 'Narration' },
  { name: 'SubtitleAgent', label: 'Subtitles' },
  { name: 'AssetAgent', label: 'Assets' },
  { name: 'VideoAgent', label: 'Video' },
]

export default function ExecutionView({ projectId, onCompleted }: Props) {
  const [steps, setSteps] = useState<PipelineStep[]>(
    PIPELINE_STEPS.map(s => ({ ...s, state: 'waiting', percent: 0 }))
  )
  const [logs, setLogs] = useState<LogEntry[]>([])
  const [_connection, setConnection] = useState<HubConnection | null>(null)

  useEffect(() => {
    const conn = new HubConnectionBuilder()
      .withUrl('/hubs/execution')
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build()

    conn.on('ProgressUpdate', (update: { agentName: string; message: string; percentComplete: number }) => {
      setSteps(prev => prev.map(s =>
        s.name === update.agentName
          ? { ...s, state: 'running', percent: update.percentComplete }
          : s
      ))
      setLogs(prev => [...prev, {
        timestamp: new Date().toLocaleTimeString(),
        agentName: update.agentName,
        message: update.message,
      }])
    })

    conn.on('AgentCompleted', (agentName: string) => {
      setSteps(prev => prev.map(s =>
        s.name === agentName ? { ...s, state: 'done', percent: 100 } : s
      ))
    })

    conn.on('AgentFailed', (agentName: string, reason: string) => {
      setSteps(prev => prev.map(s =>
        s.name === agentName ? { ...s, state: 'failed' } : s
      ))
      setLogs(prev => [...prev, {
        timestamp: new Date().toLocaleTimeString(),
        agentName,
        message: `Failed: ${reason}`,
      }])
    })

    conn.on('WorkflowCompleted', () => onCompleted())

    conn.start()
      .then(() => conn.invoke('JoinProject', projectId))
      .catch(console.error)

    setConnection(conn)
    return () => { conn.stop() }
  }, [projectId, onCompleted])

  const stateIcon = (state: PipelineStep['state']) => {
    if (state === 'done') return <i className="fa-solid fa-circle-check text-emerald-400" />
    if (state === 'running') return <i className="fa-solid fa-spinner fa-spin text-indigo-400" />
    if (state === 'failed') return <i className="fa-solid fa-circle-xmark text-red-400" />
    return <i className="fa-regular fa-circle text-slate-600" />
  }

  return (
    <div className="max-w-3xl mx-auto px-4 py-10">
      <div className="flex items-center justify-between mb-8">
        <div className="flex items-center gap-3">
          <i className="fa-solid fa-robot text-indigo-400 text-xl" />
          <h1 className="text-xl font-bold text-white">FutureBox</h1>
        </div>
        <span className="text-slate-400 text-sm font-mono">Project {projectId.slice(0, 8)}</span>
      </div>

      {/* Pipeline */}
      <div className="bg-[#1a1d27] border border-slate-700 rounded-2xl p-6 mb-6">
        <h2 className="text-xs font-semibold text-slate-400 uppercase tracking-wider mb-5">Pipeline</h2>
        <div className="space-y-4">
          {steps.map(step => (
            <div key={step.name}>
              <div className="flex items-center justify-between mb-1">
                <div className="flex items-center gap-3">
                  {stateIcon(step.state)}
                  <span className={`text-sm font-medium ${step.state === 'waiting' ? 'text-slate-500' : 'text-white'}`}>
                    {step.label}
                  </span>
                </div>
                <span className="text-xs text-slate-500 capitalize">{step.state}</span>
              </div>
              <div className="h-1 bg-slate-800 rounded-full overflow-hidden">
                <div
                  className={`h-full rounded-full transition-all duration-500 ${
                    step.state === 'done' ? 'bg-emerald-500' :
                    step.state === 'failed' ? 'bg-red-500' :
                    step.state === 'running' ? 'bg-indigo-500' : 'bg-transparent'
                  }`}
                  style={{ width: `${step.percent}%` }}
                />
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Live Log */}
      <div className="bg-[#1a1d27] border border-slate-700 rounded-2xl p-6">
        <h2 className="text-xs font-semibold text-slate-400 uppercase tracking-wider mb-4">Live Log</h2>
        <div className="space-y-2 max-h-64 overflow-y-auto font-mono text-xs">
          {logs.length === 0 && (
            <p className="text-slate-600">Waiting for agents to start...</p>
          )}
          {logs.map((log, i) => (
            <div key={i} className="flex gap-3 text-slate-300">
              <span className="text-slate-600 shrink-0">[{log.timestamp}]</span>
              <span className="text-indigo-400 shrink-0">{log.agentName}</span>
              <span>{log.message}</span>
            </div>
          ))}
        </div>
      </div>
    </div>
  )
}
