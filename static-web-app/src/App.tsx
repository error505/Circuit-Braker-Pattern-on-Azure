import React from 'react';
import './App.css';
import ApiCaller from './components/ApiCaller';

const App: React.FC = () => {
  return (
    <div className="App">
      <header className="App-header">
        <h1>Circuit Breaker Pattern - Client App</h1>
        <ApiCaller />
      </header>
    </div>
  );
};

export default App;
