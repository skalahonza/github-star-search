import { Input } from "@/components/ui/input"

interface SearchInputProps {
  query: string
  setQuery: (query: string) => void
}

export default function SearchInput({ query, setQuery }: SearchInputProps) {
  return (
    <div className="mb-6">
      <Input
        type="text"
        placeholder="Search..."
        value={query}
        onChange={(e) => setQuery(e.target.value)}
        className="w-full px-4 py-2 text-lg"
      />
    </div>
  )
}

