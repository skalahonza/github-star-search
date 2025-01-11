interface SearchResult {
  id: number
  title: string
  snippet: string
}

interface SearchResultsProps {
  results: SearchResult[]
  loading: boolean
}

export default function SearchResults({ results, loading }: SearchResultsProps) {
  if (loading) {
    return <div className="text-center">Loading...</div>
  }

  if (results.length === 0) {
    return <div className="text-center">No results found</div>
  }

  return (
    <div className="space-y-4">
      {results.map((result) => (
        <div key={result.id} className="bg-white p-4 rounded shadow">
          <h2 className="text-xl font-semibold mb-2">{result.title}</h2>
          <p className="text-gray-600">{result.snippet}</p>
        </div>
      ))}
    </div>
  )
}

