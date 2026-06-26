import { useState } from 'react'

interface Props {
  onProjectCreated: (projectId: string) => void
}

export default function NewProject({ onProjectCreated }: Props) {
  const [topic, setTopic] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!topic.trim()) return

    setLoading(true)
    setError(null)

    try {
      const res = await fetch('/api/projects', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ goal: topic }),
      })

      if (!res.ok) throw new Error('Failed to create project.')

      const data = await res.json() as { projectId: string }
      onProjectCreated(data.projectId)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Something went wrong.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="flex items-center justify-center min-h-screen px-4">
      <div className="w-full max-w-xl">
        <div className="mb-10 text-center">
          <div className="flex items-center justify-center gap-3 mb-3">
            <i className="fa-solid fa-robot text-indigo-400 text-3xl" />
            <h1 className="text-3xl font-bold text-white tracking-tight">FutureBox</h1>
          </div>
          <p className="text-slate-400 text-sm">Agent OS — you set the goal, we run the process</p>
        </div>

        <form onSubmit={handleSubmit} className="bg-[#1a1d27] border border-slate-700 rounded-2xl p-8">
          <label className="block text-slate-300 text-sm font-medium mb-3">
            What do you want to create?
          </label>
          <textarea
            className="w-full bg-[#0f1117] border border-slate-600 rounded-xl px-4 py-3 text-white placeholder-slate-500 resize-none focus:outline-none focus:border-indigo-500 transition-colors text-sm"
            rows={3}
            placeholder="e.g. A YouTube video explaining the history of Ran Online"
            value={topic}
            onChange={e => setTopic(e.target.value)}
            disabled={loading}
          />

          {error && (
            <p className="mt-3 text-red-400 text-sm flex items-center gap-2">
              <i className="fa-solid fa-circle-exclamation" />
              {error}
            </p>
          )}

          <button
            type="submit"
            disabled={loading || !topic.trim()}
            className="mt-4 w-full bg-indigo-600 hover:bg-indigo-500 disabled:bg-indigo-900 disabled:text-indigo-600 text-white font-semibold py-3 rounded-xl transition-colors flex items-center justify-center gap-2"
          >
            {loading ? (
              <>
                <i className="fa-solid fa-spinner fa-spin" />
                Starting...
              </>
            ) : (
              <>
                <i className="fa-solid fa-play" />
                Create Video
              </>
            )}
          </button>
        </form>
      </div>
    </div>
  )
}
