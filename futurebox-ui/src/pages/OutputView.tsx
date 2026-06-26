interface Props {
  projectId: string
  onNewProject: () => void
}


export default function OutputView({ projectId, onNewProject }: Props) {
  return (
    <div className="max-w-3xl mx-auto px-4 py-10">
      <div className="flex items-center justify-between mb-8">
        <div className="flex items-center gap-3">
          <i className="fa-solid fa-robot text-indigo-400 text-xl" />
          <h1 className="text-xl font-bold text-white">FutureBox</h1>
        </div>
        <span className="flex items-center gap-2 text-emerald-400 text-sm font-medium">
          <i className="fa-solid fa-circle-check" />
          Completed
        </span>
      </div>

      {/* Video preview placeholder */}
      <div className="bg-[#1a1d27] border border-slate-700 rounded-2xl p-6 mb-6">
        <div className="aspect-video bg-slate-800 rounded-xl flex items-center justify-center mb-4">
          <i className="fa-solid fa-play text-slate-600 text-4xl" />
        </div>
        <div className="flex gap-3">
          <a
            href={`/api/projects/${projectId}/download/video`}
            className="flex-1 bg-indigo-600 hover:bg-indigo-500 text-white font-semibold py-3 rounded-xl transition-colors flex items-center justify-center gap-2 text-sm"
          >
            <i className="fa-solid fa-download" />
            Download video.mp4
          </a>
          <a
            href={`/api/projects/${projectId}/logs`}
            className="flex-1 bg-slate-700 hover:bg-slate-600 text-white font-semibold py-3 rounded-xl transition-colors flex items-center justify-center gap-2 text-sm"
          >
            <i className="fa-solid fa-list" />
            View Full Log
          </a>
        </div>
      </div>

      {/* Outputs */}
      <div className="bg-[#1a1d27] border border-slate-700 rounded-2xl p-6 mb-6">
        <h2 className="text-xs font-semibold text-slate-400 uppercase tracking-wider mb-4">Outputs</h2>
        <div className="space-y-3">
          {(['video.mp4', 'script.txt', 'narration.mp3', 'subtitles.srt'] as const).map(file => (
            <div key={file} className="flex items-center gap-3 text-sm text-slate-300">
              <i className={`fa-solid ${file.endsWith('.mp4') ? 'fa-film' : file.endsWith('.mp3') ? 'fa-music' : file.endsWith('.srt') ? 'fa-closed-captioning' : 'fa-file-lines'} text-slate-500 w-4`} />
              {file}
            </div>
          ))}
        </div>
      </div>

      <button
        onClick={onNewProject}
        className="w-full bg-slate-700 hover:bg-slate-600 text-white font-semibold py-3 rounded-xl transition-colors flex items-center justify-center gap-2 text-sm"
      >
        <i className="fa-solid fa-plus" />
        New Project
      </button>
    </div>
  )
}
