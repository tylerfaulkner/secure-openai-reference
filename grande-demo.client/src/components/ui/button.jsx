import { cn } from "../../utils/cn"

export default function Button({ children, onClick, className }) {
  return (
    <button
      className={cn("bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded-sm transition-all", className)}
      onClick={onClick}
    >
      {children}
    </button>
  )
}