'use client'

import { useState, useEffect } from 'react'
import SearchInput from './components/SearchInput'
import SearchResults from './components/SearchResults'

export default function Home() {
  const [query, setQuery] = useState('')
  const [results, setResults] = useState([])
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    if (query.trim() === '') {
      setResults([])
      return
    }

    setLoading(true)
    const timeoutId = setTimeout(() => {
      // Simulating API call
      const mockResults = [
        { id: 1, title: `Result 1 for "${query}"`, snippet: 'This is a sample result...' },
        { id: 2, title: `Result 2 for "${query}"`, snippet: 'Another sample result...' },
        { id: 3, title: `Result 3 for "${query}"`, snippet: 'Yet another sample result...' },
      ]
      setResults(mockResults)
      setLoading(false)
    }, 300) // 300ms delay to simulate API call

    return () => clearTimeout(timeoutId)
  }, [query])

  return (
    <main className="min-h-screen bg-gray-100 py-8 px-4 sm:px-6 lg:px-8">
      <div className="max-w-3xl mx-auto">
        <h1 className="text-3xl font-bold text-center mb-8">Search Engine</h1>
        <SearchInput query={query} setQuery={setQuery} />
        <SearchResults results={results} loading={loading} />
      </div>
    </main>
  )
}

