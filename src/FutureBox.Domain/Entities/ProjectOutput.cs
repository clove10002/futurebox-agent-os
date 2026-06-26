namespace FutureBox.Domain.Entities;

public class ProjectOutput
{
    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public string FileName { get; private set; }
    public string FilePath { get; private set; }
    public string MimeType { get; private set; }
    public long FileSizeBytes { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private ProjectOutput() { FileName = string.Empty; FilePath = string.Empty; MimeType = string.Empty; }

    public static ProjectOutput Create(Guid projectId, string fileName, string filePath, string mimeType, long fileSizeBytes)
    {
        return new ProjectOutput
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            FileName = fileName,
            FilePath = filePath,
            MimeType = mimeType,
            FileSizeBytes = fileSizeBytes,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}
