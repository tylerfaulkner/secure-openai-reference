import { useEffect, useState } from 'react';
import './App.css';
import Button from './components/ui/button';
import AiPanel from './components/ui/ai-panel';
import xorbixLogoText from './assets/xorbix-dark-text.webp';

function App() {

    return (
        <div className="flex flex-col h-screen bg-background justify-center place-items-center relative">
            <img src={xorbixLogoText} className="absolute top-0 left-0 w-1/5 m-16"/>
            <AiPanel />
        </div>
   )
}

export default App;